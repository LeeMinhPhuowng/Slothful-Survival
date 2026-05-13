using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    private IReadOnlyList<RoomEntity> _rooms;
    private Tilemap _wallTilemap;
    private RoomEntity _currentRoom;
    private HashSet<RoomEntity> _clearedRooms = new HashSet<RoomEntity>();

    private void Awake()
    {
        Instance = this;
    }

    private float _cellSize = 1f;

    public void Initialize(IReadOnlyList<RoomEntity> rooms, Tilemap wallTilemap, float cellSize = 1f)
    {
        _rooms = rooms;
        _wallTilemap = wallTilemap;
        _cellSize = cellSize;
        
        // Start room is cleared by default
        foreach (var room in _rooms)
        {
            if (room.GetPosition() == Vector2Int.zero)
            {
                _clearedRooms.Add(room);
                break;
            }
        }
    }

    private void Update()
    {
        if (_rooms == null || _rooms.Count == 0 || PlayerInfo.instance == null) return;

        UpdateCurrentRoom();
        CheckRoomClearState();
    }

    private void UpdateCurrentRoom()
    {
        foreach (var room in _rooms)
        {
            int size = room.Config.RoomSize;
            float startX = room.GetPosition().x * size * _cellSize;
            float startY = room.GetPosition().y * size * _cellSize;
            float endX = startX + (size * _cellSize);
            float endY = startY + (size * _cellSize);

            Vector3 playerPos = PlayerInfo.instance.transform.position;
            if (playerPos.x >= startX && playerPos.x <= endX &&
                playerPos.y >= startY && playerPos.y <= endY)
            {
                if (_currentRoom != room)
                {
                    _currentRoom = room;
                    OnRoomEntered(room);
                }
                break;
            }
        }
    }

    private void OnRoomEntered(RoomEntity room)
    {
        if (!_clearedRooms.Contains(room))
        {
            // Lock doors
            SetDoors(room, true);
            // Trigger spawner
            Spawner.Instance.StartSpawningForRoom(room);
        }
    }

    private void CheckRoomClearState()
    {
        if (_currentRoom == null || _clearedRooms.Contains(_currentRoom)) return;

        if (Spawner.Instance.IsRoomCleared())
        {
            _clearedRooms.Add(_currentRoom);
            // Unlock doors
            SetDoors(_currentRoom, false);
            Debug.Log("Room Cleared!");
        }
    }

    private void SetDoors(RoomEntity room, bool isLocked)
    {
        if (_wallTilemap == null || room.Config == null) return;

        int size = room.Config.RoomSize;
        Vector2Int gridPos = room.GetPosition();
        int startX = gridPos.x * size;
        int startY = gridPos.y * size;
        int doorCenter = size / 2;

        TileBase wallTile = room.Config.TopWall; // Fallback tile

        // Check neighbours and lock/unlock doors
        if (room.GetNeighbours().ContainsKey(Vector2Int.up))
            SetTileAt(_wallTilemap, new Vector3Int(startX + doorCenter, startY + size - 1, 0), isLocked ? room.Config.TopWall : null);

        if (room.GetNeighbours().ContainsKey(Vector2Int.down))
            SetTileAt(_wallTilemap, new Vector3Int(startX + doorCenter, startY + 0, 0), isLocked ? room.Config.BottomWall : null);

        if (room.GetNeighbours().ContainsKey(Vector2Int.right))
            SetTileAt(_wallTilemap, new Vector3Int(startX + size - 1, startY + doorCenter, 0), isLocked ? room.Config.RightWall : null);

        if (room.GetNeighbours().ContainsKey(Vector2Int.left))
            SetTileAt(_wallTilemap, new Vector3Int(startX + 0, startY + doorCenter, 0), isLocked ? room.Config.LeftWall : null);
    }

    private Dictionary<Vector3Int, GameObject> _doorColliders = new Dictionary<Vector3Int, GameObject>();

    private void SetTileAt(Tilemap tilemap, Vector3Int pos, TileBase tile)
    {
        tilemap.SetTile(pos, tile);
        
        bool isLocked = tile != null;
        if (isLocked)
        {
            if (!_doorColliders.ContainsKey(pos))
            {
                var go = new GameObject("DoorCol");
                go.transform.SetParent(tilemap.transform);
                go.transform.localPosition = new Vector3((pos.x + 0.5f) * _cellSize, (pos.y + 0.5f) * _cellSize, 0);
                var col = go.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1.5f * _cellSize, 1.5f * _cellSize); // Slightly thicker to ensure no clipping
                _doorColliders[pos] = go;
            }
            _doorColliders[pos].SetActive(true);
        }
        else
        {
            if (_doorColliders.ContainsKey(pos))
            {
                _doorColliders[pos].SetActive(false);
            }
        }
    }
}
