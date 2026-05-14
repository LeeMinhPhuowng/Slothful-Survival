using Game.UI.Data;
using Game.UI.View;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class RevivePanelView : UIPanelView
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text remainingTimeText;
        [SerializeField] private Slider timerSlider;
        [SerializeField] private Button reviveButton;

        private RevivePanelViewModel _viewModel;
        private readonly CompositeDisposable _bindings = new();
        private bool _isListening;

        public override PanelId PanelId => PanelId.Revive;

        public void Bind(RevivePanelViewModel viewModel)
        {
            _viewModel = viewModel;
            AddListeners();
            RefreshStaticText();
            BindReactive();
        }

        public void Unbind()
        {
            RemoveListeners();
            _bindings.Clear();
            _viewModel = null;
        }

        public override void Show()
        {
            base.Show();
            RefreshStaticText();
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

            reviveButton?.onClick.AddListener(OnReviveClicked);
            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            reviveButton?.onClick.RemoveListener(OnReviveClicked);
            _isListening = false;
        }

        private void BindReactive()
        {
            _bindings.Clear();
            if (_viewModel == null)
            {
                return;
            }

            _viewModel.RemainingSeconds
                .Subscribe(UpdateRemainingSeconds)
                .AddTo(_bindings);

            _viewModel.TimerPercent
                .Subscribe(UpdateTimerPercent)
                .AddTo(_bindings);
        }

        private void RefreshStaticText()
        {
            if (_viewModel == null)
            {
                return;
            }

            if (messageText != null)
            {
                messageText.text = $"Revive for {_viewModel.ReviveGoldCost} gold?";
            }
        }

        private void UpdateRemainingSeconds(int seconds)
        {
            if (remainingTimeText != null)
            {
                remainingTimeText.text = seconds.ToString() + "s";
            }
        }

        private void UpdateTimerPercent(float percent)
        {
            if (timerSlider != null)
            {
                timerSlider.value = percent;
            }
        }

        private void OnReviveClicked()
        {
            _viewModel?.Revive();
        }
    }
}
