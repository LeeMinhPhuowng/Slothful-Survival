using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.Profile
{
    public sealed class ConfirmationDialogBinder : MonoBehaviour
    {
        [SerializeField] private ConfirmationDialogView dialogView;

        private ConfirmationDialogViewModel _viewModel;

        [Inject]
        private void Construct(ConfirmationDialogViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (dialogView == null)
            {
                dialogView = GetComponentInChildren<ConfirmationDialogView>(true);
            }

            if (dialogView == null)
            {
                Debug.LogError("[ConfirmationDialogBinder] ConfirmationDialogView is not assigned.");
                return;
            }

            dialogView.Bind(_viewModel);
        }

        private void OnDestroy()
        {
            dialogView?.Unbind();
        }
    }
}
