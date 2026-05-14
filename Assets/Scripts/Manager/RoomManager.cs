using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    private IReadOnlyList<RoomEntity> _rooms;
    private Tilemap _wallTilemap;
    private Tilemap _groundTilemap;
    private RoomEntity _currentRoom;
    private HashSet<RoomEntity> _clearedRooms = new HashSet<RoomEntity>();
    private Coroutine _roomEntryCoroutine;
    private bool _isSequenceRunning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (MapView.GeneratedRooms != null && MapView.WallTilemap != null)
        {
            Initialize(MapView.GeneratedRooms, MapView.WallTilemap, MapView.GroundTilemap);
            Debug.Log($"[RoomManager] Self-initialized with {MapView.GeneratedRooms.Count} rooms.");
        }
    }

    public void Initialize(IReadOnlyList<RoomEntity> rooms, Tilemap wallTilemap, Tilemap groundTilemap)
    {
        _rooms = rooms;
        _wallTilemap = wallTilemap;
        _groundTilemap = groundTilemap;
        
        foreach (var room in _rooms)
        {
            if (room.GetRoomType() == RoomType.Start)
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
        if (_wallTilemap == null) return;

        const int entryMargin = 2;

        foreach (var room in _rooms)
        {
            int size = room.Config.RoomSize;
            Vector2Int gridPos = room.GetPosition();
            int tileStartX = gridPos.x * size;
            int tileStartY = gridPos.y * size;

            Vector3 innerMin = _wallTilemap.CellToWorld(new Vector3Int(tileStartX + entryMargin, tileStartY + entryMargin, 0));
            Vector3 innerMax = _wallTilemap.CellToWorld(new Vector3Int(tileStartX + size - entryMargin, tileStartY + size - entryMargin, 0));

            Vector3 playerPos = PlayerInfo.instance.transform.position;
            if (playerPos.x >= innerMin.x && playerPos.x <= innerMax.x &&
                playerPos.y >= innerMin.y && playerPos.y <= innerMax.y)
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
            if (_roomEntryCoroutine != null)
                StopCoroutine(_roomEntryCoroutine);
            _roomEntryCoroutine = StartCoroutine(RoomEntrySequence(room));
        }
    }

    /// <summary>
    /// 1) Close doors immediately (with wall tiles drawn)
    /// 2) Wait 1 second
    /// 3) Start spawning enemies
    /// </summary>
    private IEnumerator RoomEntrySequence(RoomEntity room)
    {
        _isSequenceRunning = true;
        // Close doors with wall tiles
        SetDoors(room, true);
        Debug.Log($"[RoomManager] Doors locked for room at {room.GetPosition()}");

        // Wait 1 second before spawning
        yield return new WaitForSeconds(1f);

        // Start enemy spawning
        if (Spawner.Instance != null)
        {
            Spawner.Instance.StartSpawningForRoom(room);
            Debug.Log($"[RoomManager] Spawning started for room at {room.GetPosition()}");
        }
        _isSequenceRunning = false;
    }

    private void CheckRoomClearState()
    {
        if (_currentRoom == null || _clearedRooms.Contains(_currentRoom) || _isSequenceRunning) return;

        if (Spawner.Instance != null && Spawner.Instance.IsRoomCleared())
        {
            _clearedRooms.Add(_currentRoom);
            SetDoors(_currentRoom, false);
            Debug.Log("Room Cleared!");
        }
    }

    private int[] GetDoorOffsets(int roomSize)
    {
        int center = roomSize / 2;
        if (roomSize % 2 == 0)
        {
            return new int[] { center - 1, center };
        }
        else
        {
            return new int[] { center - 1, center, center + 1 };
        }
    }

    private void SetDoors(RoomEntity room, bool isLocked)
    {
        if (_wallTilemap == null || room.Config == null) return;

        int size = room.Config.RoomSize;
        Vector2Int gridPos = room.GetPosition();
        int startX = gridPos.x * size;
        int startY = gridPos.y * size;
        int[] doorOffsets = GetDoorOffsets(size);

        // Up door
        if (room.GetNeighbours().ContainsKey(Vector2Int.up))
        {
            foreach (int offset in doorOffsets)
            {
                Vector3Int pos = new Vector3Int(startX + offset, startY + size - 1, 0);
                _wallTilemap.SetTile(pos, isLocked ? room.Config.TopWall : null);
            }
        }

        // Down door
        if (room.GetNeighbours().ContainsKey(Vector2Int.down))
        {
            foreach (int offset in doorOffsets)
            {
                Vector3Int pos = new Vector3Int(startX + offset, startY, 0);
                _wallTilemap.SetTile(pos, isLocked ? room.Config.BottomWall : null);
            }
        }

        // Right door
        if (room.GetNeighbours().ContainsKey(Vector2Int.right))
        {
            foreach (int offset in doorOffsets)
            {
                Vector3Int pos = new Vector3Int(startX + size - 1, startY + offset, 0);
                _wallTilemap.SetTile(pos, isLocked ? room.Config.RightWall : null);
            }
        }

        // Left door
        if (room.GetNeighbours().ContainsKey(Vector2Int.left))
        {
            foreach (int offset in doorOffsets)
            {
                Vector3Int pos = new Vector3Int(startX, startY + offset, 0);
                _wallTilemap.SetTile(pos, isLocked ? room.Config.LeftWall : null);
            }
        }
    }
}
