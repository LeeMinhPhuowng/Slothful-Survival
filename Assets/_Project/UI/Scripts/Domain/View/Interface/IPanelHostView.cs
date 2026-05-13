using Game.UI.Data;

namespace Game.UI.Service
{
    public interface IPanelHost
    {
        void Show(PanelId panelId);
        void Hide(PanelId panelId);
        void HideAll();
        bool Contains(PanelId panelId);
    }
}
