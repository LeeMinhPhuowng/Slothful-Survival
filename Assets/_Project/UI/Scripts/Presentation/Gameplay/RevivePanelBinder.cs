using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class RevivePanelBinder : MonoBehaviour
    {
        [SerializeField] private RevivePanelView panelView;

        private RevivePanelViewModel _viewModel;

        [Inject]
        private void Construct(RevivePanelViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (panelView == null)
            {
                panelView = GetComponentInChildren<RevivePanelView>(true);
            }

            if (panelView == null)
            {
                Debug.LogError("[RevivePanelBinder] RevivePanelView is not assigned.");
                return;
            }

            panelView.Bind(_viewModel);
        }

        private void OnDestroy()
        {
            panelView?.Unbind();
        }
    }
}
