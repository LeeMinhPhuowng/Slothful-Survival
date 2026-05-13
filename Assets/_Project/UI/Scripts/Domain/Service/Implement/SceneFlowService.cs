using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.UI.Core;
using Game.UI.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Service
{
    public sealed class SceneFlowService : ISceneFlowService
    {
        private readonly ISceneNameRegistry _sceneNameRegistry;
        private readonly IGameCatalog _gameCatalog;
        private readonly ILoadingOverlayService _loadingOverlayService;
        private readonly float _minimumLoadingDurationSeconds;
        private SceneId _currentScene;
        private bool _isLoading = false;
        private float _loadingProgress = 0f;

        public SceneId CurrentScene => _currentScene;
        public bool IsLoading => _isLoading;
        public float LoadingProgress => _loadingProgress;
        public GameplayLoadRequest? LastGameplayLoadRequest { get; private set; }

        public SceneFlowService(
            ISceneNameRegistry sceneNameRegistry,
            IGameCatalog gameCatalog,
            ILoadingOverlayService loadingOverlayService,
            float minimumLoadingDurationSeconds)
        {
            _sceneNameRegistry = sceneNameRegistry;
            _gameCatalog = gameCatalog;
            _loadingOverlayService = loadingOverlayService;
            _minimumLoadingDurationSeconds = Mathf.Max(0f, minimumLoadingDurationSeconds);
            _currentScene = SceneId.MainMenu;
        }

        public UniTask<SceneLoadResult> LoadMainMenuAsync(CancellationToken cancellationToken = default)
        {
            return LoadSceneAsync(SceneId.MainMenu, cancellationToken);
        }

        public UniTask<SceneLoadResult> LoadChoosingMapAsync(CancellationToken cancellationToken = default)
        {
            return LoadSceneAsync(SceneId.ChoosingMap, cancellationToken);
        }

        public UniTask<SceneLoadResult> LoadGameplayAsync(GameplayLoadRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.MapId))
            {
                return UniTask.FromResult(SceneLoadResult.Failed(
                    SceneId.Gameplay,
                    SceneLoadFailureReason.MissingGameplayRequest,
                    "Gameplay load request must include a map id."));
            }

            if (string.IsNullOrWhiteSpace(request.CharacterId))
            {
                return UniTask.FromResult(SceneLoadResult.Failed(
                    SceneId.Gameplay,
                    SceneLoadFailureReason.MissingGameplayRequest,
                    "Gameplay load request must include a character id."));
            }

            LevelSO mapConfig = _gameCatalog.GetMapConfig(request.MapId);
            if (mapConfig == null)
            {
                return UniTask.FromResult(SceneLoadResult.Failed(
                    SceneId.Gameplay,
                    SceneLoadFailureReason.SceneNotFound,
                    $"Map id is not registered: {request.MapId}"));
            }

            CharacterInfoSO characterConfig = _gameCatalog.GetCharacterConfig(request.CharacterId);
            if (characterConfig == null)
            {
                return UniTask.FromResult(SceneLoadResult.Failed(
                    SceneId.Gameplay,
                    SceneLoadFailureReason.SceneNotFound,
                    $"Character id is not registered: {request.CharacterId}"));
            }

            LastGameplayLoadRequest = request;
            GameplayLaunchContext.Set(request, mapConfig, characterConfig);
            return LoadSceneAsync(SceneId.Gameplay, cancellationToken);
        }

        public UniTask<SceneLoadResult> ReloadGameplayAsync(CancellationToken cancellationToken = default)
        {
            if (!LastGameplayLoadRequest.HasValue)
            {
                return UniTask.FromResult(SceneLoadResult.Failed(
                    SceneId.Gameplay,
                    SceneLoadFailureReason.MissingGameplayRequest,
                    "No previous gameplay load request is available."));
            }

            GameplayLoadRequest previous = LastGameplayLoadRequest.Value;
            GameplayLoadRequest retryRequest = new(previous.MapId, previous.CharacterId, true);
            return LoadGameplayAsync(retryRequest, cancellationToken);
        }

        private async UniTask<SceneLoadResult> LoadSceneAsync(SceneId sceneId, CancellationToken cancellationToken)
        {
            if (_isLoading)
            {
                return SceneLoadResult.Failed(sceneId, SceneLoadFailureReason.AlreadyLoading, "A scene load is already running.");
            }

            if (!_sceneNameRegistry.TryGetSceneName(sceneId, out string sceneName))
            {
                return SceneLoadResult.Failed(sceneId, SceneLoadFailureReason.SceneNotFound, $"Scene name is not registered for {sceneId}.");
            }

            _isLoading = true;
            SetProgress(0f);
            _loadingOverlayService.Show();
            float loadingStartedAt = Time.realtimeSinceStartup;

            try
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
                if (operation == null)
                {
                    return SceneLoadResult.Failed(sceneId, SceneLoadFailureReason.SceneNotFound, $"Unity could not start loading scene '{sceneName}'.");
                }

                operation.allowSceneActivation = false;
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                while (operation.progress < 0.9f)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    SetProgress(operation.progress / 0.9f);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                SetProgress(1f);
                await WaitForMinimumLoadingDurationAsync(loadingStartedAt, cancellationToken);

                operation.allowSceneActivation = true;
                while (!operation.isDone)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                _currentScene = sceneId;
                return SceneLoadResult.Succeeded(sceneId);
            }
            catch (OperationCanceledException)
            {
                return SceneLoadResult.Failed(sceneId, SceneLoadFailureReason.Cancelled, "Scene load was cancelled.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return SceneLoadResult.Failed(sceneId, SceneLoadFailureReason.Exception, exception.Message);
            }
            finally
            {
                _isLoading = false;
                _loadingOverlayService.Hide();
            }
        }

        private void SetProgress(float progress)
        {
            float clampedProgress = Mathf.Clamp01(progress);
            _loadingProgress = clampedProgress;
            _loadingOverlayService.SetProgress(clampedProgress);
        }

        private UniTask WaitForMinimumLoadingDurationAsync(float loadingStartedAt, CancellationToken cancellationToken)
        {
            float elapsed = Time.realtimeSinceStartup - loadingStartedAt;
            float remaining = _minimumLoadingDurationSeconds - elapsed;

            if (remaining <= 0f)
            {
                return UniTask.CompletedTask;
            }

            return UniTask.Delay(
                TimeSpan.FromSeconds(remaining),
                DelayType.UnscaledDeltaTime,
                PlayerLoopTiming.Update,
                cancellationToken);
        }
    }
}
