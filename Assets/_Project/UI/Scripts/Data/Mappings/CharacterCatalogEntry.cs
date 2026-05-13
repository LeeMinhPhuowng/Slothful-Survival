using System;
using Game.UI.Model;

namespace Game.UI.Data
{
    [Serializable]
    public sealed class CharacterCatalogEntry
    {
        public string CharacterId;
        public string DisplayName;
        public CharacterInfoSO Config;
        public bool IsUnlocked = true;
        public int Level;

        public string ResolveId()
        {
            return string.IsNullOrWhiteSpace(CharacterId) && Config != null ? Config.name : CharacterId;
        }

        public CharacterModel ToModel()
        {
            string id = ResolveId();
            return new CharacterModel
            {
                CharacterId = id,
                DisplayName = string.IsNullOrWhiteSpace(DisplayName) ? id : DisplayName,
                Config = Config,
                IsUnlocked = IsUnlocked,
                Level = Level
            };
        }
    }
}