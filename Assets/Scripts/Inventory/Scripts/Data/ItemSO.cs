using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Serialization;

namespace Features.Inventory
{
    public enum ItemType
    {
        Bag,
        Regular
    }

    /// <summary>
    /// Item shape & rotation definition (ScriptableObject data).
    /// </summary>
    [CreateAssetMenu(menuName = "Inventory/Item")]
    public class ItemSO : ScriptableObject
    {
        public ItemType itemType = ItemType.Regular;
        public string nameString;

        [FormerlySerializedAs("prefab")]
        [Tooltip("The 2D UI Prefab representing this item on the inventory grid Canvas")]
        public Transform visualPrefab;

        public int width;
        public int height;

        #region Direction Helpers

        public enum Dir
        {
            Down,
            Left,
            Up,
            Right,
        }

        public static Dir GetNextDir(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return Dir.Left;
                case Dir.Left: return Dir.Up;
                case Dir.Up: return Dir.Right;
                case Dir.Right: return Dir.Down;
            }
        }

        public static Vector2Int GetDirForwardVector(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return new Vector2Int(0, -1);
                case Dir.Left: return new Vector2Int(-1, 0);
                case Dir.Up: return new Vector2Int(0, +1);
                case Dir.Right: return new Vector2Int(+1, 0);
            }
        }

        public static Dir GetDir(Vector2Int from, Vector2Int to)
        {
            if (from.x < to.x) return Dir.Right;
            if (from.x > to.x) return Dir.Left;
            if (from.y < to.y) return Dir.Up;
            return Dir.Down;
        }

        #endregion

        #region Rotation & Grid Positions

        public int GetRotationAngle(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return 0;
                case Dir.Left: return 90;
                case Dir.Up: return 180;
                case Dir.Right: return 270;
            }
        }

        public Vector2Int GetRotationOffset(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return new Vector2Int(0, 0);
                case Dir.Left: return new Vector2Int(0, width);
                case Dir.Up: return new Vector2Int(width, height);
                case Dir.Right: return new Vector2Int(height, 0);
            }
        }

        public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            switch (dir)
            {
                default:
                case Dir.Down:
                case Dir.Up:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
                case Dir.Left:
                case Dir.Right:
                    for (int x = 0; x < height; x++)
                    {
                        for (int y = 0; y < width; y++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
            }
            return gridPositionList;
        }

        #endregion
    }
}
