using System;
using Game.UI.Data;

namespace Game.UI.Save
{
    [Serializable]
    public sealed class EquipmentSlotSaveEntry
    {
        public EquipmentSlot Slot;
        public string ItemId;
    }
}
