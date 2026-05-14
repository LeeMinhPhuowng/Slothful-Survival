using System;
using DG.Tweening;
using Game.UI.Data;
using Game.UI.Model;
using Game.UI.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class RunResultPanelView : UIPanelView
    {
        [SerializeField] private PanelId panelId = PanelId.Win;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text enemyKilledText;
        [SerializeField] private TMP_Text playingTimeText;
        [SerializeField] private TMP_Text goldEarnedText;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField, Min(0.05f)] private float countDuration = 0.75f;

        private RunResultPanelViewModel _viewModel;
        private bool _isListening;
        private Sequence _sequence;

        public override PanelId PanelId => panelId;

        public void Bind(RunResultPanelViewModel viewModel)
        {
            _viewModel = viewModel;
            AddListeners();
            if (gameObject.activeInHierarchy)
            {
                Refresh();
            }
        }

        public void Unbind()
        {
            RemoveListeners();
            KillTween();
            _viewModel = null;
        }

        public override void Show()
        {
            base.Show();
            Refresh();
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void AddListeners()
        {
            if (_isListening)
            {
                return;
            }

            homeButton?.onClick.AddListener(OnHomeClicked);
            tryAgainButton?.onClick.AddListener(OnTryAgainClicked);
            nextLevelButton?.onClick.AddListener(OnNextLevelClicked);
            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            homeButton?.onClick.RemoveListener(OnHomeClicked);
            tryAgainButton?.onClick.RemoveListener(OnTryAgainClicked);
            nextLevelButton?.onClick.RemoveListener(OnNextLevelClicked);
            _isListening = false;
        }

        private void Refresh()
        {
            RunResultModel result = _viewModel?.CurrentResult;
            if (result == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = result.IsWin ? "Victory" : "Defeat";
            }

            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(result.IsWin);
            }

            AnimateResult(result);
        }

        private void AnimateResult(RunResultModel result)
        {
            KillTween();

            int seconds = Mathf.Max(0, Mathf.RoundToInt((float)result.PlayingTime.TotalSeconds));
            _sequence = DOTween.Sequence().SetUpdate(true);

            AppendCounter(enemyKilledText, result.EnemyDefeated, value => $"Enemies: {value}");
            AppendCounter(playingTimeText, seconds, value => $"Time: {FormatSeconds(value)}");
            AppendCounter(goldEarnedText, result.GoldEarned, value => $"Gold: {value}");
        }

        private void AppendCounter(TMP_Text target, int endValue, Func<int, string> formatter)
        {      
            if (target == null)
            {
                return;
            }
            
            _sequence.Join(DOVirtual.Int(0, endValue, countDuration, value => target.text = formatter(value)));
        }

        private static string FormatSeconds(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }

        private void KillTween()
        {
            _sequence?.Kill();
            _sequence = null;
        }

        private void OnHomeClicked()
        {
            _viewModel?.Home();
        }

        private void OnTryAgainClicked()
        {
            _viewModel?.TryAgain();
        }

        private void OnNextLevelClicked()
        {
            _viewModel?.NextLevel();
        }
    }
}
