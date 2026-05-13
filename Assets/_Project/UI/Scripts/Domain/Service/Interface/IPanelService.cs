using Game.UI.Data;

namespace Game.UI.Service
{
    public interface IPanelService
    {
        PanelId? CurrentPanel { get; }
        bool HasOpenPanel { get; }

        void Open(PanelId panelId);
        void Close(PanelId panelId);
        void CloseTop();
        void CloseAll();
        bool IsOpen(PanelId panelId);
    }
}