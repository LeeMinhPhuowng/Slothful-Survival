using Reflex.Attributes;
using UnityEngine;

namespace Game.UI.Presentation.Profile
{
    public sealed class EquipmentItemDetailPanelBinder : MonoBehaviour
    {
        [SerializeField] private EquipmentItemDetailPanelView panelView;

        private EquipmentItemDetailPanelViewModel _viewModel;

        [Inject]
        private void Construct(EquipmentItemDetailPanelViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            if (panelView == null)
            {
                panelView = GetComponentInChildren<EquipmentItemDetailPanelView>(true);
            }

            if (panelView == null)
            {
                Debug.LogError("[EquipmentItemDetailPanelBinder] EquipmentItemDetailPanelView is not assigned.");
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
