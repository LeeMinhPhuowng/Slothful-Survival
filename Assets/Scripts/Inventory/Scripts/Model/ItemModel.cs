using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Pure data model for an item placed in the inventory.
    /// No MonoBehaviour, no visual references.
    /// </summary>
    public class ItemModel
    {
        public ItemSO ItemSO { get; private set; }
        public Vector2Int Origin { get; private set; }
        public ItemSO.Dir Dir { get; private set; }

        public ItemModel(ItemSO itemSO, Vector2Int origin, ItemSO.Dir dir)
        {
            ItemSO = itemSO;
            Origin = origin;
            Dir = dir;
        }

        public void SetOrigin(Vector2Int newOrigin)
        {
            Origin = newOrigin;
        }

        public void SetDir(ItemSO.Dir newDir)
        {
            Dir = newDir;
        }

        public List<Vector2Int> GetGridPositionList()
        {
            return ItemSO.GetGridPositionList(Origin, Dir);
        }

        public override string ToString()
        {
            return ItemSO != null ? ItemSO.nameString : "null";
        }

        #region Save/Load

        public SaveData ToSaveData()
        {
            return new SaveData
            {
                itemSOName = ItemSO.name,
                origin = Origin,
                dir = Dir
            };
        }

        [Serializable]
        public class SaveData
        {
            public string itemSOName;
            public Vector2Int origin;
            public ItemSO.Dir dir;
        }

        #endregion
    }
}
