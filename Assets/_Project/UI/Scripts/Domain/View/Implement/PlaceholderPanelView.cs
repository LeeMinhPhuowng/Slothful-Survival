using Game.UI.Data;
using Game.UI.Service;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.View
{
    public sealed class PlaceholderPanelView : UIPanelView
    {
        [SerializeField] private PanelId panelId;
        [SerializeField] private Button backButton;

        private IPanelService _panelService;

        public override PanelId PanelId => panelId;

        [Inject]
        public void Construct(IPanelService panelService)
        {
            _panelService = panelService;
        }

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }
        }

        private void OnBackClicked()
        {
            _panelService.Close(PanelId);
        }
    }
}
