using System;
using Game.UI.Data;
using Game.UI.View;

namespace Game.UI.Core
{
    [Serializable]
    public sealed class PanelEntry
    {
        public PanelId PanelId;
        public UIPanelView PanelView;
    }
}