using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    private List<EnemyWaveSO> _enemyWaves = new List<EnemyWaveSO>();
    private IReadOnlyList<RoomEntity> _rooms;
    private RoomEntity _currentRoom;
    private Coroutine _spawnCoroutine;
    private bool _isSpawningWave = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Self-initialize from MapView's static data (waves come from LevelSO via MapView)
        if (MapView.GeneratedRooms != null)
        {
            _rooms = MapView.GeneratedRooms;

            if (MapView.EnemyWaves != null && MapView.EnemyWaves.Count > 0)
            {
                _enemyWaves.Clear();
                _enemyWaves.AddRange(MapView.EnemyWaves);
            }

            Debug.Log($"[Spawner] Initialized: {_rooms.Count} rooms, {_enemyWaves.Count} enemy waves from LevelSO.");
        }
    }

    public void StartSpawningForRoom(RoomEntity room)
    {
        _currentRoom = room;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }
        _spawnCoroutine = StartCoroutine(SpawnEnemyWaves());
    }

    public bool IsRoomCleared()
    {
        return !_isSpawningWave && GameManager.EnemyCount <= 0;
    }

    IEnumerator SpawnEnemyWaves()
    {
        _isSpawningWave = true;
        foreach (var wave in _enemyWaves)
        {
            foreach (var waveInfo in wave.waveInfos)
            {
                for (int i = 0; i < waveInfo.amount; i++)
                {
                    Vector3 spawnPos = GetSpawnPosition();
                    ObjectPool.instance.SpawnFromPool(waveInfo.type, spawnPos);
                }
            }
            yield return new WaitForSeconds(wave.timeTillNextWave);
        }
        _isSpawningWave = false;
    }

    Vector3 GetSpawnPosition()
    {
        if (_currentRoom == null)
        {
            if (PlayerInfo.instance != null) return PlayerInfo.instance.transform.position + (Vector3)Random.insideUnitCircle * 5f;
            return Vector3.zero;
        }

        int size = _currentRoom.Config.RoomSize;
        Vector2Int gridPos = _currentRoom.GetPosition();
        int tileStartX = gridPos.x * size;
        int tileStartY = gridPos.y * size;

        Tilemap tilemap = MapView.WallTilemap;
        if (tilemap != null)
        {
            // Pick a random side: 0: Top, 1: Bottom, 2: Left, 3: Right
            int side = Random.Range(0, 4);
            int margin = 2; // Offset from the actual wall to avoid spawning inside it
            
            int spawnTileX = 0;
            int spawnTileY = 0;

            switch (side)
            {
                case 0: // Top
                    spawnTileX = Random.Range(tileStartX + margin, tileStartX + size - margin);
                    spawnTileY = tileStartY + size - margin;
                    break;
                case 1: // Bottom
                    spawnTileX = Random.Range(tileStartX + margin, tileStartX + size - margin);
                    spawnTileY = tileStartY + margin;
                    break;
                case 2: // Left
                    spawnTileX = tileStartX + margin;
                    spawnTileY = Random.Range(tileStartY + margin, tileStartY + size - margin);
                    break;
                case 3: // Right
                    spawnTileX = tileStartX + size - margin;
                    spawnTileY = Random.Range(tileStartY + margin, tileStartY + size - margin);
                    break;
            }

            return tilemap.CellToWorld(new Vector3Int(spawnTileX, spawnTileY, 0));
        }

        if (PlayerInfo.instance != null) return PlayerInfo.instance.transform.position + (Vector3)Random.insideUnitCircle * 3f;
        return Vector3.zero;
    }
}
