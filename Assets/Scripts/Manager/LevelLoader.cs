using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] Transform mapContainer;
    private LevelSO level;

    void Start()
    {
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        level = CoverFlow.instance.GetLevelSO();
        levelName.text = level.levelName;
    }

    public void OnPlayButtonClicked()
    {
        Spawner.Instance.InitializeEnemyWaves(level.enemyWaves);
        
        // Create Grid and Tilemaps
        GameObject gridObj = new GameObject("Grid");
        gridObj.transform.SetParent(mapContainer);
        var grid = gridObj.AddComponent<Grid>();
        if (level.mapConfig != null)
        {
            grid.cellSize = new Vector3(level.mapConfig.CellSize, level.mapConfig.CellSize, 0);
        }

        GameObject groundObj = new GameObject("Ground");
        groundObj.transform.SetParent(gridObj.transform);
        Tilemap groundTilemap = groundObj.AddComponent<Tilemap>();
        groundObj.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();

        GameObject wallObj = new GameObject("Wall");
        wallObj.transform.SetParent(gridObj.transform);
        Tilemap wallTilemap = wallObj.AddComponent<Tilemap>();
        var wallRenderer = wallObj.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();
        wallRenderer.sortingOrder = 1; // Wall above ground

        // Remove TilemapCollider2D from here, we will create BoxColliders manually
        var wallRb = wallObj.AddComponent<Rigidbody2D>();
        wallRb.bodyType = RigidbodyType2D.Static;

        if (level.mapConfig != null)
        {
            MapEntity mapEntity = new MapEntity(level.mapConfig);
            mapEntity.GenerateMap();
            foreach (var roomEntity in mapEntity.GetRooms())
            {
                RoomView.Draw(roomEntity, groundTilemap, wallTilemap);
                CreateRoomColliders(roomEntity, wallObj.transform);
            }
            
            // Pass the generated rooms to the Spawner
            Spawner.Instance.SetRooms(mapEntity.GetRooms());

            // Add and initialize RoomManager
            var roomManager = gridObj.AddComponent<RoomManager>();
            roomManager.Initialize(mapEntity.GetRooms(), wallTilemap, level.mapConfig.CellSize);

            // Set Player spawn position to the center of the root room
            if (mapEntity.GetRooms().Count > 0)
            {
                var rootRoom = mapEntity.GetRooms()[0];
                if (rootRoom.Config != null)
                {
                    int size = rootRoom.Config.RoomSize;
                    float cs = level.mapConfig.CellSize;
                    PlayerSetter.instance.SetSpawnPosition(new Vector3(size / 2f * cs, size / 2f * cs, 0f));
                }
            }
        }

        this.gameObject.SetActive(false);
    }

    private void CreateRoomColliders(RoomEntity room, Transform parent)
    {
        if (room.Config == null) return;
        int size = room.Config.RoomSize;
        Vector2Int gridPos = room.GetPosition();
        float startX = gridPos.x * size;
        float startY = gridPos.y * size;
        int doorCenter = size / 2;
        float thickness = 1f;
        float cs = level.mapConfig != null ? level.mapConfig.CellSize : 1f;

        // Helper to add collider
        void AddBox(float x, float y, float w, float h)
        {
            var go = new GameObject("WallCol");
            go.transform.SetParent(parent);
            go.transform.localPosition = new Vector3((startX + x + w / 2f) * cs, (startY + y + h / 2f) * cs, 0);
            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(w * cs, h * cs);
            // Thicker inner border to prevent clipping
            if (w == size || w > 1f) col.size = new Vector2(w * cs, (h + 0.5f) * cs); // Horizontal
            if (h == size || h > 1f) col.size = new Vector2((w + 0.5f) * cs, h * cs); // Vertical
        }

        // Top Wall
        if (room.GetNeighbours().ContainsKey(Vector2Int.up))
        {
            AddBox(0, size - 1, doorCenter, thickness);
            AddBox(doorCenter + 1, size - 1, size - doorCenter - 1, thickness);
        }
        else AddBox(0, size - 1, size, thickness);

        // Bottom Wall
        if (room.GetNeighbours().ContainsKey(Vector2Int.down))
        {
            AddBox(0, 0, doorCenter, thickness);
            AddBox(doorCenter + 1, 0, size - doorCenter - 1, thickness);
        }
        else AddBox(0, 0, size, thickness);

        // Left Wall
        if (room.GetNeighbours().ContainsKey(Vector2Int.left))
        {
            AddBox(0, 0, thickness, doorCenter);
            AddBox(0, doorCenter + 1, thickness, size - doorCenter - 1);
        }
        else AddBox(0, 0, thickness, size);

        // Right Wall
        if (room.GetNeighbours().ContainsKey(Vector2Int.right))
        {
            AddBox(size - 1, 0, thickness, doorCenter);
            AddBox(size - 1, doorCenter + 1, thickness, size - doorCenter - 1);
        }
        else AddBox(size - 1, 0, thickness, size);
    }
}
