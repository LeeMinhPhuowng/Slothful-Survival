using System.Collections.Generic;
using Game.UI.Model;

namespace Game.UI.Service
{
    public interface IEquipmentCatalog
    {
        EquipmentItemModel GetItem(string itemId);
        IReadOnlyList<EquipmentItemModel> GetAllItems();
    }
}
