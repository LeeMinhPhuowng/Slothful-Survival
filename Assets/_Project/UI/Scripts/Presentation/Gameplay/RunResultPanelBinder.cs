using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class RunResultPanelBinder : MonoBehaviour
    {
        [SerializeField] private RunResultPanelView panelView;

        private RunResultPanelViewModel _viewModel;

        [Inject]
        private void Construct(RunResultPanelViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (panelView == null)
            {
                panelView = GetComponentInChildren<RunResultPanelView>(true);
            }

            if (panelView == null)
            {
                Debug.LogError("[RunResultPanelBinder] RunResultPanelView is not assigned.");
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
