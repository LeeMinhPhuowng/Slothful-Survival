using Game.UI.Service;

namespace Game.UI.Presentation.Profile
{
    public sealed class ConfirmationDialogViewModel
    {
        private readonly IConfirmationDialogService _confirmationDialogService;

        public string Title => _confirmationDialogService.Title;
        public string Message => _confirmationDialogService.Message;

        public ConfirmationDialogViewModel(IConfirmationDialogService confirmationDialogService)
        {
            _confirmationDialogService = confirmationDialogService;
        }

        public void Confirm()
        {
            _confirmationDialogService.Confirm();
        }

        public void Cancel()
        {
            _confirmationDialogService.Cancel();
        }
    }
}
