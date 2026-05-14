using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.UI.Data;
using Game.UI.Model;
using R3;
using UnityEngine;

namespace Game.UI.Service
{
    public sealed class GameRunService : IGameRunService, IDisposable
    {
        private readonly IPanelService _panelService;
        private readonly ISceneFlowService _sceneFlowService;
        private readonly IWalletService _walletService;
        private readonly IMapSelectionService _mapSelectionService;
        private readonly IPlayerProgressService _progressService;
        private readonly ReactiveProperty<bool> _isPaused = new(false);
        private readonly ReactiveProperty<RunResultModel> _currentRunResult = new ReactiveProperty<RunResultModel>(null);
        private readonly ReactiveProperty<int> _reviveRemainingSeconds;
        private readonly ReactiveProperty<float> _reviveTimerPercent;
        private readonly int _targetEnemyKills;
        private readonly int _reviveDurationSeconds;
        private readonly float _startedAt;

        private CancellationTokenSource _reviveCountdownCts;
        private int _enemyKilled;
        private bool _isPlayerDead;
        private bool _isRunFinished;

        public ReadOnlyReactiveProperty<bool> IsPaused => _isPaused;
        public ReadOnlyReactiveProperty<RunResultModel> CurrentRunResult => _currentRunResult;
        public ReadOnlyReactiveProperty<int> ReviveRemainingSeconds => _reviveRemainingSeconds;
        public ReadOnlyReactiveProperty<float> ReviveTimerPercent => _reviveTimerPercent;
        public int ReviveGoldCost { get; }

        public GameRunService(
            IPanelService panelService,
            ISceneFlowService sceneFlowService,
            IWalletService walletService,
            IMapSelectionService mapSelectionService,
            IPlayerProgressService progressService,
            int reviveGoldCost = 100,
            int reviveDurationSeconds = 10)
        {
            _panelService = panelService;
            _sceneFlowService = sceneFlowService;
            _walletService = walletService;
            _mapSelectionService = mapSelectionService;
            _progressService = progressService;
            ReviveGoldCost = Math.Max(0, reviveGoldCost);
            _reviveDurationSeconds = Math.Max(1, reviveDurationSeconds);
            _reviveRemainingSeconds = new ReactiveProperty<int>(_reviveDurationSeconds);
            _reviveTimerPercent = new ReactiveProperty<float>(1f);
            _targetEnemyKills = CountEnemies(GameplayLaunchContext.MapConfig);
            _startedAt = Time.realtimeSinceStartup;

            GameplayRunSignals.EnemyKilled += OnEnemyKilled;
            GameplayRunSignals.PlayerDied += OnPlayerDied;
        }

        public void Pause()
        {
            if (_isPaused.Value)
            {
                _panelService.Open(PanelId.Pause);
                return;
            }

            Time.timeScale = 0f;
            _isPaused.Value = true;
            _panelService.Open(PanelId.Pause);
        }

        public void Resume()
        {
            if (!_isPaused.Value)
            {
                _panelService.Close(PanelId.Pause);
                return;
            }

            Time.timeScale = 1f;
            _isPaused.Value = false;
            _panelService.Close(PanelId.Pause);
        }

        public void TryAgain()
        {
            TryAgainAsync().Forget();
        }

        public void NextLevel()
        {
            NextLevelAsync().Forget();
        }

        public void ReturnToChoosingMap()
        {
            ReturnToChoosingMapAsync().Forget();
        }

        public void ReviveByGold()
        {
            if (!_isPlayerDead || _isRunFinished)
            {
                return;
            }

            if (!_walletService.TrySpend(CurrencyType.Gold, ReviveGoldCost))
            {
                Debug.LogWarning($"[GameRunService] Not enough gold to revive. Cost: {ReviveGoldCost}");
                return;
            }

            _reviveCountdownCts?.Cancel();
            _isPlayerDead = false;
            if (PlayerInfo.instance != null)
            {
                PlayerInfo.instance.Revive(0.5f);
            }
            _panelService.Close(PanelId.Revive);
            Time.timeScale = 1f;
            _isPaused.Value = false;
        }

        public void Dispose()
        {
            GameplayRunSignals.EnemyKilled -= OnEnemyKilled;
            GameplayRunSignals.PlayerDied -= OnPlayerDied;
            _reviveCountdownCts?.Cancel();
            _reviveCountdownCts?.Dispose();
            Time.timeScale = 1f;
            _isPaused.Dispose();
            _currentRunResult.Dispose();
            _reviveRemainingSeconds.Dispose();
            _reviveTimerPercent.Dispose();
        }

        private async UniTask TryAgainAsync()
        {
            ResumeBeforeSceneChange();
            SceneLoadResult result = await _sceneFlowService.ReloadGameplayAsync();
            if (!result.Success)
            {
                Debug.LogError($"[GameRunService] Failed to retry gameplay: {result.FailureReason} - {result.Message}");
            }
        }

        private async UniTask ReturnToChoosingMapAsync()
        {
            ResumeBeforeSceneChange();
            SceneLoadResult result = await _sceneFlowService.LoadChoosingMapAsync();
            if (!result.Success)
            {
                Debug.LogError($"[GameRunService] Failed to return to ChoosingMap: {result.FailureReason} - {result.Message}");
            }
        }

        private void ResumeBeforeSceneChange()
        {
            _reviveCountdownCts?.Cancel();
            Time.timeScale = 1f;
            _isPaused.Value = false;
            _panelService.CloseAll();
        }

        private async UniTask NextLevelAsync()
        {
            ResumeBeforeSceneChange();

            GameplayLoadRequest? nextRequest = CreateNextLevelRequest();
            if (!nextRequest.HasValue)
            {
                SceneLoadResult choosingMapResult = await _sceneFlowService.LoadChoosingMapAsync();
                if (!choosingMapResult.Success)
                {
                    Debug.LogError($"[GameRunService] Failed to return to ChoosingMap: {choosingMapResult.FailureReason} - {choosingMapResult.Message}");
                }

                return;
            }

            SceneLoadResult result = await _sceneFlowService.LoadGameplayAsync(nextRequest.Value);
            if (!result.Success)
            {
                Debug.LogError($"[GameRunService] Failed to load next level: {result.FailureReason} - {result.Message}");
            }
        }

        private void OnEnemyKilled()
        {
            if (_isRunFinished)
            {
                return;
            }

            _enemyKilled++;
            if (_targetEnemyKills > 0 && _enemyKilled >= _targetEnemyKills)
            {
                CompleteRun(true);
            }
        }

        private void OnPlayerDied()
        {
            if (_isRunFinished || _isPlayerDead)
            {
                return;
            }

            _isPlayerDead = true;
            Time.timeScale = 0f;
            _isPaused.Value = true;
            _panelService.Open(PanelId.Revive);
            StartReviveCountdown().Forget();
        }

        private async UniTaskVoid StartReviveCountdown()
        {
            _reviveCountdownCts?.Cancel();
            _reviveCountdownCts?.Dispose();
            _reviveCountdownCts = new CancellationTokenSource();
            CancellationToken cancellationToken = _reviveCountdownCts.Token;

            float remaining = _reviveDurationSeconds;
            _reviveRemainingSeconds.Value = _reviveDurationSeconds;
            _reviveTimerPercent.Value = 1f;

            try
            {
                while (remaining > 0f)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    remaining -= Time.unscaledDeltaTime;
                    _reviveRemainingSeconds.Value = Mathf.CeilToInt(Mathf.Max(0f, remaining));
                    _reviveTimerPercent.Value = Mathf.Clamp01(remaining / _reviveDurationSeconds);
                }

                if (_isPlayerDead && !_isRunFinished)
                {
                    CompleteRun(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void CompleteRun(bool isWin)
        {
            if (_isRunFinished)
            {
                return;
            }

            _isRunFinished = true;
            _reviveCountdownCts?.Cancel();

            int goldEarned = _enemyKilled * UnityEngine.Random.Range(3, 8);
            _walletService.Add(CurrencyType.Gold, goldEarned, isWin ? "run_win" : "run_lose");

            if (isWin)
            {
                UnlockNextMap();
            }

            _currentRunResult.Value = new RunResultModel
            {
                EnemyDefeated = _enemyKilled,
                GoldEarned = goldEarned,
                IsWin = isWin,
                PlayingTime = TimeSpan.FromSeconds(Mathf.Max(0f, Time.realtimeSinceStartup - _startedAt))
            };

            Time.timeScale = 0f;
            _isPaused.Value = true;
            _panelService.Close(PanelId.Revive);
            _panelService.Open(isWin ? PanelId.Win : PanelId.Lose);
        }

        private GameplayLoadRequest? CreateNextLevelRequest()
        {
            GameplayLoadRequest? currentRequest = _sceneFlowService.LastGameplayLoadRequest ?? GameplayLaunchContext.CurrentRequest;
            if (!currentRequest.HasValue)
            {
                return null;
            }

            int currentIndex = FindMapIndex(currentRequest.Value.MapId);
            if (currentIndex < 0 || currentIndex + 1 >= _mapSelectionService.Maps.Count)
            {
                return null;
            }

            MapModel nextMap = _mapSelectionService.Maps[currentIndex + 1];
            if (nextMap == null)
            {
                return null;
            }

            if (!nextMap.IsUnlocked)
            {
                _progressService.SetMapUnlocked(nextMap.MapId, true);
                nextMap.IsUnlocked = true;
            }

            return new GameplayLoadRequest(nextMap.MapId, currentRequest.Value.CharacterId, false);
        }

        private void UnlockNextMap()
        {
            GameplayLoadRequest? currentRequest = _sceneFlowService.LastGameplayLoadRequest ?? GameplayLaunchContext.CurrentRequest;
            if (!currentRequest.HasValue)
            {
                return;
            }

            int currentIndex = FindMapIndex(currentRequest.Value.MapId);
            if (currentIndex < 0 || currentIndex + 1 >= _mapSelectionService.Maps.Count)
            {
                return;
            }

            MapModel nextMap = _mapSelectionService.Maps[currentIndex + 1];
            if (nextMap == null)
            {
                return;
            }

            nextMap.IsUnlocked = true;
            _progressService.SetMapUnlocked(nextMap.MapId, true);
        }

        private int FindMapIndex(string mapId)
        {
            for (int i = 0; i < _mapSelectionService.Maps.Count; i++)
            {
                if (_mapSelectionService.Maps[i].MapId == mapId)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int CountEnemies(LevelSO level)
        {
            if (level?.enemyWaves == null)
            {
                return 0;
            }

            return level.enemyWaves
                .Where(wave => wave?.waveInfos != null)
                .SelectMany(wave => wave.waveInfos)
                .Where(info => info != null)
                .Sum(info => Math.Max(0, info.amount));
        }
    }
}
