namespace Game.UI.Data
{
    public readonly struct EquipmentChangedPayload
    {
        public readonly string CharacterId;
        public readonly EquipmentSlot Slot;
        public readonly string PreviousItemId;
        public readonly string NewItemId;

        public EquipmentChangedPayload(string characterId, EquipmentSlot slot, string previousItemId, string newItemId)
        {
            CharacterId = characterId;
            Slot = slot;
            PreviousItemId = previousItemId;
            NewItemId = newItemId;
        }
    }
}