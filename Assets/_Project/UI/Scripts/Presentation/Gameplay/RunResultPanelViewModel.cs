using Cysharp.Threading.Tasks;
using Game.UI.Model;
using Game.UI.Service;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class RunResultPanelViewModel
    {
        private readonly IGameRunService _gameRunService;

        public RunResultPanelViewModel(IGameRunService gameRunService)
        {
            _gameRunService = gameRunService;
        }

        public RunResultModel CurrentResult => _gameRunService.CurrentRunResult.CurrentValue;

        public void Home()
        {
            _gameRunService.ReturnToChoosingMap();
        }

        public void TryAgain()
        {
            _gameRunService.TryAgain();
        }

        public void NextLevel()
        {
            _gameRunService.NextLevel();
        }
    }
}
