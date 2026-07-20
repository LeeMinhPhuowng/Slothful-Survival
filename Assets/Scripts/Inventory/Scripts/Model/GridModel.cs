using System;
using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Generic 2D grid data structure. Pure C# – no MonoBehaviour dependency.
    /// </summary>
    public class GridModel<TGridObject>
    {
        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;

        public class OnGridObjectChangedEventArgs : EventArgs
        {
            public int x;
            public int y;
        }

        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly Vector3 _originPos;
        private readonly TGridObject[,] _gridArray;

        public GridModel(int width, int height, float cellSize, Vector3 originPos,
            Func<GridModel<TGridObject>, int, int, TGridObject> createGridObject)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPos = originPos;

            _gridArray = new TGridObject[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }

        public int GetWidth() => _width;
        public int GetHeight() => _height;
        public float GetCellSize() => _cellSize;

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * _cellSize + _originPos;
        }

        public void GetXY(Vector3 worldPos, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPos - _originPos).x / _cellSize);
            y = Mathf.FloorToInt((worldPos - _originPos).y / _cellSize);
        }

        public void SetGridObject(int x, int y, TGridObject value)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;
                TriggerGridObjectChanged(x, y);
            }
        }

        public void TriggerGridObjectChanged(int x, int y)
        {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }

        public void SetGridObject(Vector3 worldPos, TGridObject value)
        {
            GetXY(worldPos, out int x, out int y);
            SetGridObject(x, y, value);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                return _gridArray[x, y];
            }
            return default;
        }

        public TGridObject GetGridObject(Vector3 worldPos)
        {
            GetXY(worldPos, out int x, out int y);
            return GetGridObject(x, y);
        }

        public bool IsValidGridPosition(Vector2Int gridPosition)
        {
            int x = gridPosition.x;
            int y = gridPosition.y;
            return x >= 0 && y >= 0 && x < _width && y < _height;
        }
    }
}
