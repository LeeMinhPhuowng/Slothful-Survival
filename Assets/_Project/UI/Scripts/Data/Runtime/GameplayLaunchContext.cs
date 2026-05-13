using UnityEngine;

namespace Game.UI.Data
{
    public static class GameplayLaunchContext
    {
        public static GameplayLoadRequest? CurrentRequest { get; private set; }
        public static LevelSO MapConfig { get; private set; }
        public static CharacterInfoSO CharacterConfig { get; private set; }

        public static bool HasRequest => CurrentRequest.HasValue;

        public static void Set(GameplayLoadRequest request, LevelSO mapConfig, CharacterInfoSO characterConfig)
        {
            CurrentRequest = request;
            MapConfig = mapConfig;
            CharacterConfig = characterConfig;
        }

        public static void Clear()
        {
            CurrentRequest = null;
            MapConfig = null;
            CharacterConfig = null;
        }
    }
}
