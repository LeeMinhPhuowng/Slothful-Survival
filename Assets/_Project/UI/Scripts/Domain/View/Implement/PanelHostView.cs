using Game.UI.Core;
using Game.UI.Data;
using UnityEngine;

namespace Game.UI.Service
{
    public sealed class PanelHostView : MonoBehaviour, IPanelHost
    {
        [SerializeField] private PanelRegistry panelRegistry;

        private void Awake()
        {
            HideAll();
        }

        public void Show(PanelId panelId)
        {
            if (!panelRegistry.TryGetPanel(panelId, out var panel))
            {
                Debug.LogError($"[PanelHostView] Panel not found: {panelId}");
                return;
            }

            panel.Show();
            panel.transform.SetAsLastSibling();
        }

        public void Hide(PanelId panelId)
        {
            if (!panelRegistry.TryGetPanel(panelId, out var panel))
            {
                return;
            }

            panel.Hide();
        }

        public void HideAll()
        {
            if (panelRegistry == null) 
            {
                return;
            }

            foreach (var panel in panelRegistry.GetAllPanels())
            {
                panel.Hide();
            }
        }

        public bool Contains(PanelId panelId)
        {
            return panelRegistry != null && panelRegistry.Contains(panelId);
        }
    }
}