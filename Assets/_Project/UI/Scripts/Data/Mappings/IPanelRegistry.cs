using Game.UI.Data;
using Game.UI.View;

namespace Game.UI.Core
{
    public interface IPanelRegistry
    {
        bool TryGetPanel(PanelId panelId, out UIPanelView uIPanelView);
        bool Contains(PanelId panelId);
    }
}   