using Game.UI.Service;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class GameplayViewModel
    {
        private readonly IGameRunService _gameRunService;

        public GameplayViewModel(IGameRunService gameRunService)
        {
            _gameRunService = gameRunService;
        }

        public void OpenPause()
        {
            _gameRunService.Pause();
        }
    }
}
