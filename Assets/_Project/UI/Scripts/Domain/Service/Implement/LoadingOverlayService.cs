using Game.UI.View;
using UnityEngine;

namespace Game.UI.Service
{
    public sealed class LoadingOverlayService : ILoadingOverlayService
    {
        private readonly LoadingPanelView _loadingView;

        public bool IsVisible { get; private set; }
        public float Progress { get; private set; }

        public LoadingOverlayService(LoadingPanelView loadingView)
        {
            _loadingView = loadingView;
            Hide();
        }

        public void Show()
        {
            Progress = 0f;
            IsVisible = true;

            if (_loadingView == null) 
            {
                return;
            }
           
            _loadingView.SetVisible(true);
            _loadingView.SetProgress(Progress);
        }

        public void SetProgress(float progress)
        {
            Progress = Mathf.Clamp01(progress);
            
            if (_loadingView == null) return;

            _loadingView.SetProgress(Progress);
        }

        public void Hide()
        {
            Progress = 1f;
            IsVisible = false;

            if (_loadingView == null) 
            {
                return;
            }

            _loadingView.SetProgress(Progress);
            _loadingView.SetVisible(false);
        }
    }
}
