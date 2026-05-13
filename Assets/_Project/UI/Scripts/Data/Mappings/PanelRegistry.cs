using System.Collections.Generic;
using Game.UI.Data;
using Game.UI.View;
using UnityEngine;

namespace Game.UI.Core
{
    public sealed class PanelRegistry : MonoBehaviour, IPanelRegistry
    {
        [SerializeField] private List<PanelEntry> panelEntries = new();
        private readonly Dictionary<PanelId, UIPanelView> _panelId2UIView = new();
        private bool _isBuilt;

        private void Awake()
        {
            BuildPanelMap();
        }

        public bool TryGetPanel(PanelId panelId, out UIPanelView panelView)
        {
            EnsureBuilt();
            return _panelId2UIView.TryGetValue(panelId, out panelView);
        }

        public bool Contains(PanelId panelId)
        {
            EnsureBuilt();
            return _panelId2UIView.ContainsKey(panelId);
        }

        public IReadOnlyCollection<UIPanelView> GetAllPanels()
        {
            EnsureBuilt();
            return _panelId2UIView.Values;
        }

        private void EnsureBuilt()
        {
            if (_isBuilt)
            {
                return;
            }

            BuildPanelMap();
        }

        private void BuildPanelMap()
        {
            _panelId2UIView.Clear();

            foreach (PanelEntry entry in panelEntries)
            {
                if (entry == null || entry.PanelView == null)
                {
                    Debug.LogWarning("[PanelRegistry] Empty panel entry.");
                    continue;
                }

                if (_panelId2UIView.ContainsKey(entry.PanelId))
                {
                    Debug.LogError($"[PanelRegistry] Duplicate panel id: {entry.PanelId}");
                    continue;
                }

                _panelId2UIView.Add(entry.PanelId, entry.PanelView);
            }

            _isBuilt = true;
        }
    }
}
