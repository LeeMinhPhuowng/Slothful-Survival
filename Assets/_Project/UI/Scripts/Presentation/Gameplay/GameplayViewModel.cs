using Cysharp.Threading.Tasks;
using Game.UI.Data;
using Game.UI.Service;
using UnityEngine;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class GameplayViewModel
    {
        private readonly IPanelService _panelService;
        private readonly ISceneFlowService _sceneFlowService;

        public GameplayViewModel(IPanelService panelService, ISceneFlowService sceneFlowService)
        {
            _panelService = panelService;
            _sceneFlowService = sceneFlowService;
        }

        public void OpenPause()
        {
            _panelService.Open(PanelId.Pause);
        }
    }
}
