using System.Collections.Generic;
using UnityEngine;

public class RoomEntity
{
    private Vector2Int _position;
    private Dictionary<Vector2Int, RoomEntity> _neighbours;
    private RoomType _roomType;

    public RoomConfigSO Config { get; private set; }

    public RoomEntity(Vector2Int position)
    {
        _position = position;
        _neighbours = new Dictionary<Vector2Int, RoomEntity>();
        _roomType = RoomType.Default; // Default type
    }

    public Vector2Int GetPosition() => _position;
    
    public RoomType GetRoomType() => _roomType;
    public void SetRoomType(RoomType type) => _roomType = type;

    public void SetConfig(RoomConfigSO config)
    {
        Config = config;
    }

    public void AddConnection(Vector2Int direction, RoomEntity room)
    {
        if (!_neighbours.ContainsKey(direction))
        {
            _neighbours.Add(direction, room);
        }
    }
    
    public Dictionary<Vector2Int, RoomEntity> GetNeighbours() => _neighbours;
}
