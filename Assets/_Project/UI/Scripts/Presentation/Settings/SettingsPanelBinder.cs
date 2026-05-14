using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.Settings
{
    public sealed class SettingsPanelBinder : MonoBehaviour
    {
        [SerializeField] private SettingsPanelView panelView;

        private SettingsPanelViewModel _viewModel;

        [Inject]
        private void Construct(SettingsPanelViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (panelView == null)
            {
                panelView = GetComponentInChildren<SettingsPanelView>(true);
            }

            if (panelView == null)
            {
                Debug.LogError("[SettingsPanelBinder] SettingsPanelView is not assigned.");
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
