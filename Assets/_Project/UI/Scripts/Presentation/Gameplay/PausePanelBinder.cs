using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class PausePanelBinder : MonoBehaviour
    {
        [SerializeField] private PausePanelView panelView;

        private PausePanelViewModel _viewModel;

        [Inject]
        private void Construct(PausePanelViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (panelView == null)
            {
                panelView = GetComponentInChildren<PausePanelView>(true);
            }

            if (panelView == null)
            {
                Debug.LogError("[PausePanelBinder] PausePanelView is not assigned.");
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
