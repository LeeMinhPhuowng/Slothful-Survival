using Game.UI.Data;
using Game.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class PausePanelView : UIPanelView
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private Button returnToMapButton;
        [SerializeField] private Button closeButton;

        private PausePanelViewModel _viewModel;
        private bool _isListening;

        public override PanelId PanelId => PanelId.Pause;

        public void Bind(PausePanelViewModel viewModel)
        {
            _viewModel = viewModel;
            AddListeners();
        }

        public void Unbind()
        {
            RemoveListeners();
            _viewModel = null;
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

            resumeButton?.onClick.AddListener(OnResumeClicked);
            settingsButton?.onClick.AddListener(OnSettingsClicked);
            tryAgainButton?.onClick.AddListener(OnTryAgainClicked);
            returnToMapButton?.onClick.AddListener(OnReturnToMapClicked);
            closeButton?.onClick.AddListener(OnResumeClicked);
            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            resumeButton?.onClick.RemoveListener(OnResumeClicked);
            settingsButton?.onClick.RemoveListener(OnSettingsClicked);
            tryAgainButton?.onClick.RemoveListener(OnTryAgainClicked);
            returnToMapButton?.onClick.RemoveListener(OnReturnToMapClicked);
            closeButton?.onClick.RemoveListener(OnResumeClicked);
            _isListening = false;
        }

        private void OnResumeClicked()
        {
            _viewModel?.Resume();
        }

        private void OnSettingsClicked()
        {
            _viewModel?.OpenSettings();
        }

        private void OnTryAgainClicked()
        {
            _viewModel?.TryAgain();
        }

        private void OnReturnToMapClicked()
        {
            _viewModel?.ReturnToChoosingMap();
        }
    }
}
