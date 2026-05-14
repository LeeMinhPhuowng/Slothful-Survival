using Game.UI.Service;
using R3;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class RevivePanelViewModel
    {
        private readonly IGameRunService _gameRunService;

        public RevivePanelViewModel(IGameRunService gameRunService)
        {
            _gameRunService = gameRunService;
        }

        public ReadOnlyReactiveProperty<int> RemainingSeconds => _gameRunService.ReviveRemainingSeconds;
        public ReadOnlyReactiveProperty<float> TimerPercent => _gameRunService.ReviveTimerPercent;
        public int ReviveGoldCost => _gameRunService.ReviveGoldCost;

        public void Revive()
        {
            _gameRunService.ReviveByGold();
        }
    }
}
