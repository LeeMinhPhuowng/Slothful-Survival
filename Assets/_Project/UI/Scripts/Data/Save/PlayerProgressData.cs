using System;
using System.Collections.Generic;

namespace Game.UI.Save
{
    [Serializable]
    public sealed class PlayerProgressData
    {
        public int Version = 1;
        public int Gold;
        public int Diamond;
        public int CurrentLevel = 1;
        public float ExpPercent;
        public string SelectedCharacterId;
        public string SelectedMapId;
        public List<string> UnlockedEquipmentIds = new();
        public List<EquipmentSlotSaveEntry> EquippedItems = new();
        public List<string> UnlockedCharacterIds = new();
        public List<string> UnlockedMapIds = new();
        public List<string> CompletedMapIds = new();
    }
}
