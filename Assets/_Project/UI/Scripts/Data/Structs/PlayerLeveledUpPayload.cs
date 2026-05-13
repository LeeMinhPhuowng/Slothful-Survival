using System.Collections.Generic;

namespace Game.UI.Data
{
    public readonly struct PlayerLeveledUpPayload
    {
        public readonly string CharacterId;
        public readonly int NewLevel;
        public readonly IReadOnlyList<string> AugmentOptionIds;

        public PlayerLeveledUpPayload(string characterId, int newLevel, IReadOnlyList<string> augmentOptionIds)
        {
            CharacterId = characterId;
            NewLevel = newLevel;
            AugmentOptionIds = augmentOptionIds;
        }
    }
}