using System;

namespace Game.UI.Service
{
    public interface IConfirmationDialogService
    {
        string Title { get; }
        string Message { get; }
        void Request(string title, string message, Action onConfirm, Action onCancel = null);
        void Confirm();
        void Cancel();
    }
}
