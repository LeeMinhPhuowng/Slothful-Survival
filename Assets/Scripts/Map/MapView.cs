using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapView : MonoBehaviour
{
    [SerializeField] private MapConfigSO _mapConfig;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _wallTilemap;

    /// <summary>Static spawn position calculated from the Start room center. Read by PlayerSetter.</summary>
    public static Vector3? HomeRoomSpawnPosition { get; private set; }

    /// <summary>Generated map data for other systems to pick up in Start().</summary>
    public static IReadOnlyList<RoomEntity> GeneratedRooms { get; private set; }
    public static Tilemap WallTilemap { get; private set; }
    public static Tilemap GroundTilemap { get; private set; }
    public static float MapCellSize { get; private set; }
    public static List<EnemyWaveSO> EnemyWaves { get; private set; }

    private void Awake()
    {
        GenerateAndDisplayMap();
    }

    [ContextMenu("Regenerate Map")]
    public void GenerateAndDisplayMap()
    {
        Debug.Log("[MapView] === GenerateAndDisplayMap START ===");

        if (_groundTilemap != null) _groundTilemap.ClearAllTiles();
        if (_wallTilemap != null) _wallTilemap.ClearAllTiles();

        MapConfigSO activeConfig = _mapConfig;

        var launchMapConfig = Game.UI.Data.GameplayLaunchContext.MapConfig;
        if (launchMapConfig != null && launchMapConfig.mapConfig != null)
        {
            activeConfig = launchMapConfig.mapConfig;
        }
        
        if (activeConfig == null)
        {
            Debug.LogError("[MapView] No MapConfigSO available to generate map. ABORTING.");
            return;
        }

        Debug.Log($"[MapView] Config: {activeConfig.name}, Rooms={activeConfig.NumberOfRooms}, CellSize={activeConfig.CellSize}");
        MapEntity mapEntity = new MapEntity(activeConfig);
        mapEntity.GenerateMap();
        Debug.Log($"[MapView] Generated {mapEntity.GetRooms().Count} rooms.");

        foreach (var roomEntity in mapEntity.GetRooms())
        {
            RoomView.Draw(roomEntity, _groundTilemap, _wallTilemap);
        }

        // Store generated data as static fields for RoomManager / Spawner to pick up in Start()
        GeneratedRooms = mapEntity.GetRooms();
        WallTilemap = _wallTilemap;
        GroundTilemap = _groundTilemap;
        MapCellSize = activeConfig.CellSize;
        EnemyWaves = launchMapConfig != null ? launchMapConfig.enemyWaves : null;

        // Calculate center of Start room for player spawn
        var startRoom = GeneratedRooms.FirstOrDefault(r => r.GetRoomType() == RoomType.Start);
        if (startRoom == null) startRoom = GeneratedRooms[0];

        int roomSize = startRoom.Config != null ? startRoom.Config.RoomSize : 20;
        int centerTileX = startRoom.GetPosition().x * roomSize + roomSize / 2;
        int centerTileY = startRoom.GetPosition().y * roomSize + roomSize / 2;
        HomeRoomSpawnPosition = _groundTilemap.CellToWorld(new Vector3Int(centerTileX, centerTileY, 0));
        Debug.Log($"[MapView] HomeRoomSpawnPosition: {HomeRoomSpawnPosition.Value}");

        Debug.Log("[MapView] === GenerateAndDisplayMap COMPLETE ===");
    }
}
