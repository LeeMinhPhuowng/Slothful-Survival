using System.Collections.Generic;
using Game.UI.Data;
using UnityEngine;

namespace Game.UI.Service
{
    public sealed class PanelService : IPanelService
    {
        private readonly IPanelHost _panelHost;
        private readonly Stack<PanelId> _panelStack = new();

        public PanelId? CurrentPanel { get; private set; }
        public bool HasOpenPanel => _panelStack.Count > 0;

        public PanelService(IPanelHost panelHost)
        {
            _panelHost = panelHost;
        }

        public void Open(PanelId panelId)
        {
            if (!_panelHost.Contains(panelId))
            {
                Debug.LogError($"[PanelService] Panel not found: {panelId}");
                return;
            }

            if (IsOpen(panelId))
            {
                BringToTop(panelId);
                return;
            }

            _panelHost.Show(panelId);
            _panelStack.Push(panelId);
            CurrentPanel = panelId;
        }

        public void Close(PanelId panelId)
        {
            if (!IsOpen(panelId))
            {
                return;
            }

            _panelHost.Hide(panelId);
            RebuildStackWithout(panelId);
            CurrentPanel = _panelStack.Count > 0 ? _panelStack.Peek() : null;
        }

        public void CloseTop()
        {
            if (_panelStack.Count == 0)
            {
                return;
            }

            PanelId topPanel = _panelStack.Pop();
            _panelHost.Hide(topPanel);
            CurrentPanel = _panelStack.Count > 0 ? _panelStack.Peek() : null;
        }

        public void CloseAll()
        {
            _panelHost.HideAll();
            _panelStack.Clear();
            CurrentPanel = null;
        }

        public bool IsOpen(PanelId panelId)
        {
            return _panelStack.Contains(panelId);
        }

        private void BringToTop(PanelId panelId)
        {
            _panelHost.Show(panelId);
            RebuildStackWithout(panelId);
            _panelStack.Push(panelId);
            CurrentPanel = panelId;
        }

        private void RebuildStackWithout(PanelId panelId)
        {
            if (_panelStack.Count == 0)
            {
                return;
            }

            PanelId[] panels = _panelStack.ToArray();
            _panelStack.Clear();

            for (int i = panels.Length - 1; i >= 0; i--)
            {
                if (panels[i] == panelId)
                {
                    continue;
                }

                _panelStack.Push(panels[i]);
            }
        }
    }
}