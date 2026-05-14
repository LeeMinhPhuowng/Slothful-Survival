using Game.UI.Data;
using Game.UI.Service;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class PausePanelViewModel
    {
        private readonly IGameRunService _gameRunService;
        private readonly IPanelService _panelService;

        public PausePanelViewModel(IGameRunService gameRunService, IPanelService panelService)
        {
            _gameRunService = gameRunService;
            _panelService = panelService;
        }

        public void Resume()
        {
            _gameRunService.Resume();
        }

        public void OpenSettings()
        {
            _panelService.Open(PanelId.Settings);
        }

        public void TryAgain()
        {
            _gameRunService.TryAgain();
        }

        public void ReturnToChoosingMap()
        {
            _gameRunService.ReturnToChoosingMap();
        }
    }
}
