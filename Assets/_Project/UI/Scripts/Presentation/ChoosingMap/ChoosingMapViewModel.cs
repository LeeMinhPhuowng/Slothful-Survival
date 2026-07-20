using Cysharp.Threading.Tasks;
using Game.UI.Data;
using Game.UI.Model;
using Game.UI.Service;
using UnityEngine;

namespace Game.UI.Presentation.ChoosingMap
{
    public sealed class ChoosingMapViewModel
    {
        private readonly IPanelService _panelService;
        private readonly ISceneFlowService _sceneFlowService;
        private readonly IMapSelectionService _mapSelectionService;
        private readonly ICharacterRosterService _characterRosterService;
        private bool _isLoadMainMenu;
        private bool _isLoadGameplay;

        public ChoosingMapViewModel(
            IPanelService panelService,
            ISceneFlowService sceneFlowService,
            IMapSelectionService mapSelectionService,
            ICharacterRosterService characterRosterService)
        {
            _panelService = panelService;
            _sceneFlowService = sceneFlowService;
            _mapSelectionService = mapSelectionService;
            _characterRosterService = characterRosterService;
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

        public async UniTask MainMenuAsync()
        {
            if (_isLoadMainMenu)
            {
                return;
            }

            _isLoadMainMenu = true;

            try
            {
                SceneLoadResult result = await _sceneFlowService.LoadMainMenuAsync();
                if (!result.Success)
                {
                    Debug.LogError($"[ChoosingMapViewModel] Failed to load MainMenu: {result.FailureReason} - {result.Message}");
                }
            }
            finally
            {
                _isLoadMainMenu = false;
            }
        }

        public async UniTask GameplayAsync()
        {
            if (_isLoadGameplay)
            {
                return;
            }

            _isLoadGameplay = true;

            try
            {
                MapModel selectedMap = _mapSelectionService.GetSelectedMap();
                CharacterModel selectedCharacter = _characterRosterService.GetSelectedCharacter();

                if (selectedMap == null)
                {
                    Debug.LogError("[ChoosingMapViewModel] Cannot play because no map is selected.");
                    return;
                }

                if (selectedCharacter == null)
                {
                    Debug.LogError("[ChoosingMapViewModel] Cannot play because no character is selected.");
                    return;
                }

                GameplayLoadRequest request = new(selectedMap.MapId, selectedCharacter.CharacterId, false);
                SceneLoadResult result = await _sceneFlowService.LoadGameplayInventoryAsync(request);
                if (!result.Success)
                {
                    Debug.LogError($"[ChoosingMapViewModel] Failed to load Gameplay_Inventory: {result.FailureReason} - {result.Message}");
                }
            }
            finally
            {
                _isLoadGameplay = false;
            }
        }
    }
}
