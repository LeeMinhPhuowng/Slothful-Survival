using Game.UI.Data;
using Game.UI.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Profile
{
    public sealed class ConfirmationDialogView : UIPanelView
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button closeButton;

        private ConfirmationDialogViewModel _viewModel;
        private bool _isListening;

        public override PanelId PanelId => PanelId.Confirmation;

        public void Bind(ConfirmationDialogViewModel viewModel)
        {
            _viewModel = viewModel;
            AddListeners();
            Refresh();
        }

        public void Unbind()
        {
            RemoveListeners();
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

            confirmButton?.onClick.AddListener(OnConfirmClicked);
            cancelButton?.onClick.AddListener(OnCancelClicked);
            closeButton?.onClick.AddListener(OnCancelClicked);
            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            confirmButton?.onClick.RemoveListener(OnConfirmClicked);
            cancelButton?.onClick.RemoveListener(OnCancelClicked);
            closeButton?.onClick.RemoveListener(OnCancelClicked);
            _isListening = false;
        }

        private void Refresh()
        {
            if (_viewModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = _viewModel.Title;
            }

            if (messageText != null)
            {
                messageText.text = _viewModel.Message;
            }
        }

        private void OnConfirmClicked()
        {
            _viewModel?.Confirm();
        }

        private void OnCancelClicked()
        {
            _viewModel?.Cancel();
        }
    }
}
