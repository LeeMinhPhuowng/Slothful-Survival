using UnityEngine;
using UnityEngine.Tilemaps;

public class MapView : MonoBehaviour
{
    [SerializeField] private MapConfigSO _mapConfig;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _wallTilemap;

    private void Start()
    {
        GenerateAndDisplayMap();
    }

    [ContextMenu("Regenerate Map")]
    public void GenerateAndDisplayMap()
    {
        if (_groundTilemap != null) _groundTilemap.ClearAllTiles();
        if (_wallTilemap != null) _wallTilemap.ClearAllTiles();

        MapEntity mapEntity = new MapEntity(_mapConfig);
        mapEntity.GenerateMap();

        foreach (var roomEntity in mapEntity.GetRooms())
        {
            RoomView.Draw(roomEntity, _groundTilemap, _wallTilemap);
        }
    }
}
