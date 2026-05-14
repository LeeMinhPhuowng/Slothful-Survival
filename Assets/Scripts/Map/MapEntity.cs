
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapEntity
{
    private readonly List<RoomEntity> _rooms;
    private readonly MapConfigSO _config;

    public MapEntity(MapConfigSO config)
    {
        _config = config;
        _rooms = new List<RoomEntity>();
    }

    public IReadOnlyList<RoomEntity> GetRooms() => _rooms;

    private T GetRandomConfig<T>(List<T> configs) where T : RoomConfigSO
    {
        if (configs == null || configs.Count == 0) return null;
        return configs[Random.Range(0, configs.Count)];
    }

    public void GenerateMap()
    {
        var grid = new Dictionary<Vector2Int, RoomEntity>();
        var queue = new Queue<RoomEntity>();

        var rootRoom = new RoomEntity(Vector2Int.zero);
        rootRoom.SetRoomType(RoomType.Start);
        rootRoom.SetConfig(GetRandomConfig(_config.DefaultRoomConfigs));
        grid[Vector2Int.zero] = rootRoom;   
        queue.Enqueue(rootRoom);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0 && grid.Count < _config.NumberOfRooms)
        {
            var currentRoom = queue.Dequeue();

            // Shuffle directions array for random branching
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int temp = directions[i];
                int randomIndex = Random.Range(i, directions.Length);
                directions[i] = directions[randomIndex];
                directions[randomIndex] = temp;
            }

            int doorNumber = Random.Range(1, 5);

            for (int i = 0; i < doorNumber; i++)
            {
                if (grid.Count >= _config.NumberOfRooms) break;

                Vector2Int direction = directions[i];
                Vector2Int newPos = currentRoom.GetPosition() + direction;

                if (!grid.ContainsKey(newPos))
                {
                    var newRoom = new RoomEntity(newPos);
                    newRoom.SetConfig(GetRandomConfig(_config.DefaultRoomConfigs));
                    grid[newPos] = newRoom;

                    currentRoom.AddConnection(direction, newRoom);
                    newRoom.AddConnection(-direction, currentRoom);

                    queue.Enqueue(newRoom);
                }
            }
        }

        AssignRoomTypes(grid.Values.ToList());
        _rooms.AddRange(grid.Values);
    }

    private void AssignRoomTypes(List<RoomEntity> allRooms)
    {
        if (allRooms.Count <= 1) return;

        // Find the boss room (furthest from origin)
        RoomEntity bossRoom = allRooms.OrderByDescending(r => Vector2Int.Distance(Vector2Int.zero, r.GetPosition())).First();
        bossRoom.SetRoomType(RoomType.Boss);
        bossRoom.SetConfig(GetRandomConfig(_config.BossRoomConfigs));

        int totalRooms = _config.NumberOfRooms;
        
        // Calculate number of chest and shop rooms (at least 1 if possible)
        int numChests = Mathf.Max(1, Mathf.RoundToInt(totalRooms * _config.ChestRoomRatio));
        int numShops = Mathf.Max(1, Mathf.RoundToInt(totalRooms * _config.ShopRoomRatio));

        int assignedChests = 0;
        int assignedShops = 0;

        // Helper function to ensure special rooms are not too close to each other
        bool IsFarEnough(RoomEntity candidate)
        {
            // Do not spawn too close to start (at least 2 units away)
            if (Vector2Int.Distance(Vector2Int.zero, candidate.GetPosition()) < 2f)
                return false;

            // Do not spawn near Boss, Chests or Shops (at least 2 units away)
            foreach (var room in allRooms)
            {
                if (room.GetRoomType() != RoomType.Default && room.GetPosition() != candidate.GetPosition())
                {
                    // Distance < 2 means adjacent or diagonally adjacent
                    if (Vector2Int.Distance(room.GetPosition(), candidate.GetPosition()) < 2f)
                        return false;
                }
            }
            return true;
        }

        // Get all default rooms except the starting room (0,0)
        var candidates = allRooms.Where(r => r.GetPosition() != Vector2Int.zero && r.GetRoomType() == RoomType.Default).ToList();

        // Shuffle candidates for random distribution
        for (int i = 0; i < candidates.Count; i++)
        {
            RoomEntity temp = candidates[i];
            int randomIndex = Random.Range(i, candidates.Count);
            candidates[i] = candidates[randomIndex];
            candidates[randomIndex] = temp;
        }

        // Phase 1: Try to assign special rooms with strict distance constraints
        foreach (var room in candidates)
        {
            if (assignedChests >= numChests && assignedShops >= numShops) break;

            if (IsFarEnough(room))
            {
                if (assignedChests < numChests)
                {
                    room.SetRoomType(RoomType.Chest);
                    room.SetConfig(GetRandomConfig(_config.ChestRoomConfigs));
                    assignedChests++;
                }
                else if (assignedShops < numShops)
                {
                    room.SetRoomType(RoomType.Shop);
                    room.SetConfig(GetRandomConfig(_config.ShopRoomConfigs));
                    assignedShops++;
                }
            }
        }

        // Phase 2: Fallback. If map is too small and we couldn't assign enough, relax constraints.
        if (assignedChests < numChests || assignedShops < numShops)
        {
            foreach (var room in candidates)
            {
                if (assignedChests >= numChests && assignedShops >= numShops) break;

                if (room.GetRoomType() == RoomType.Default)
                {
                    if (assignedChests < numChests)
                    {
                        room.SetRoomType(RoomType.Chest);
                        room.SetConfig(GetRandomConfig(_config.ChestRoomConfigs));
                        assignedChests++;
                    }
                    else if (assignedShops < numShops)
                    {
                        room.SetRoomType(RoomType.Shop);
                        room.SetConfig(GetRandomConfig(_config.ShopRoomConfigs));
                        assignedShops++;
                    }
                }
            }
        }
    }
}
