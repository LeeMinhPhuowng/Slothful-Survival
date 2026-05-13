namespace Game.UI.Data
{
    public readonly struct PanelChangedPayload
    {
        public readonly PanelId PanelId;
        public readonly bool IsOpen;

        public PanelChangedPayload(PanelId panelId, bool isOpen)
        {
            PanelId = panelId;
            IsOpen = isOpen;
        }
    }
}