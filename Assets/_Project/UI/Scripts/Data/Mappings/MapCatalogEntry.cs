using System;
using Game.UI.Model;

namespace Game.UI.Data
{
    [Serializable]
    public sealed class MapCatalogEntry
    {
        public string MapId;
        public string DisplayName;
        public LevelSO Config;
        public bool IsUnlocked = true;
        public bool IsCompleted;

        public string ResolveId()
        {
            return string.IsNullOrWhiteSpace(MapId) && Config != null ? Config.name : MapId;
        }

        public MapModel ToModel()
        {
            string id = ResolveId();
            return new MapModel
            {
                MapId = id,
                DisplayName = string.IsNullOrWhiteSpace(DisplayName) ? Config != null ? Config.levelName : id : DisplayName,
                Config = Config,
                IsUnlocked = IsUnlocked,
                IsCompleted = IsCompleted
            };
        }
    }
}