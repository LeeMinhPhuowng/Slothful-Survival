namespace Game.UI.Service
{
    public interface ILoadingOverlayService
    {
        bool IsVisible { get; }
        float Progress { get; }

        void Show();
        void SetProgress(float progress);
        void Hide();
    }
}