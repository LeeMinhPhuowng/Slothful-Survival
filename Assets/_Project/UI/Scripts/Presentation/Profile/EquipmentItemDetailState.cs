namespace Game.UI.Presentation.Profile
{
    public sealed class EquipmentItemDetailState
    {
        public string SelectedItemId { get; private set; }

        public void Select(string itemId)
        {
            SelectedItemId = itemId;
        }

        public void Clear()
        {
            SelectedItemId = null;
        }
    }
}
