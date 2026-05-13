using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.Profile
{
    public sealed class ProfilePanelBinder : MonoBehaviour
    {
        [SerializeField] private ProfilePanelView panelView;

        private ProfilePanelViewModel _viewModel;

        [Inject]
        private void Construct(ProfilePanelViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (panelView == null)
            {
                panelView = GetComponentInChildren<ProfilePanelView>(true);
            }

            if (panelView == null)
            {
                Debug.LogError("[ProfilePanelBinder] ProfilePanelView is not assigned.");
                return;
            }

            panelView.Bind(_viewModel);
        }

        private void OnDestroy()
        {
            if (panelView != null)
            {
                panelView.Unbind();
            }
        }
    }
}
