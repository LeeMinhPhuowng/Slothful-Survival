using System;
using Game.UI.Data;

namespace Game.UI.Service
{
    public sealed class ConfirmationDialogService : IConfirmationDialogService
    {
        private readonly IPanelService _panelService;
        private Action _onConfirm;
        private Action _onCancel;

        public string Title { get; private set; }
        public string Message { get; private set; }

        public ConfirmationDialogService(IPanelService panelService)
        {
            _panelService = panelService;
        }

        public void Request(string title, string message, Action onConfirm, Action onCancel = null)
        {
            Title = title;
            Message = message;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            _panelService.Open(PanelId.Confirmation);
        }

        public void Confirm()
        {
            Action action = _onConfirm;
            Clear();
            _panelService.Close(PanelId.Confirmation);
            action?.Invoke();
        }

        public void Cancel()
        {
            Action action = _onCancel;
            Clear();
            _panelService.Close(PanelId.Confirmation);
            action?.Invoke();
        }

        private void Clear()
        {
            Title = string.Empty;
            Message = string.Empty;
            _onConfirm = null;
            _onCancel = null;
        }
    }
}
