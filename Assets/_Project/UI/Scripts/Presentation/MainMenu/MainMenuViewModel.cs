using Cysharp.Threading.Tasks;
using Game.UI.Data;
using Game.UI.Service;
using UnityEngine;

namespace Game.UI.Presentation.MainMenu
{
    public sealed class MainMenuViewModel
    {
        private readonly IPanelService _panelService;
        private readonly ISceneFlowService _sceneFlowService;
        private bool _isLoadChoosingMap;

        public MainMenuViewModel(IPanelService panelService, ISceneFlowService sceneFlowService)
        {
            _panelService = panelService;
            _sceneFlowService = sceneFlowService;
        }

        public void OpenSettings()
        {
            _panelService.Open(PanelId.Settings);
        }

        public void OpenShop()
        {
            _panelService.Open(PanelId.Shop);
        }

        public void OpenProfile()
        {
            _panelService.Open(PanelId.Profile);
        }

        public async UniTask ChooseMapAsync()
        {
            if (_isLoadChoosingMap)
            {
                return;
            }

            _isLoadChoosingMap = true;

            try
            {
                SceneLoadResult result = await _sceneFlowService.LoadChoosingMapAsync();
                if (!result.Success)
                {
                    Debug.LogError($"[MainMenuViewModel] Failed to load ChoosingMap: {result.FailureReason} - {result.Message}");
                }
            }
            finally
            {
                _isLoadChoosingMap = false;
            }
        }
    }
}
