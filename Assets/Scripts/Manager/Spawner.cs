    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;
    using System.Linq;

    public class Spawner : MonoBehaviour
    {
        public static Spawner Instance;
        
        [Header("Enemy Waves")]
        [SerializeField] List<EnemyWaveSO> enemyWaves = new List<EnemyWaveSO>();
        
        private IReadOnlyList<RoomEntity> _rooms;
        private RoomEntity _currentRoom;
        private Coroutine _spawnCoroutine;
        private bool _isSpawningWave = false;
        private float _cellSize = 1f;

        private void Awake()
        {
            Instance = this;
        }

        public void SetRooms(IReadOnlyList<RoomEntity> rooms, float cellSize = 1f)
        {
            _rooms = rooms;
            _cellSize = cellSize;
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
            foreach (var wave in enemyWaves)
            {
                foreach (var waveInfo in wave.waveInfos)
                {
                    for(int i = 0; i < waveInfo.amount; i++)
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
            float startX = _currentRoom.GetPosition().x * size * _cellSize;
            float startY = _currentRoom.GetPosition().y * size * _cellSize;

            // Spawn randomly inside the room, keeping a margin from walls
            float randomX = Random.Range(startX + (1.5f * _cellSize), startX + (size * _cellSize) - (1.5f * _cellSize));
            float randomY = Random.Range(startY + (1.5f * _cellSize), startY + (size * _cellSize) - (1.5f * _cellSize));

            return new Vector3(randomX, randomY, 0f);
        }

        public void InitializeEnemyWaves(List<EnemyWaveSO> waves)
        {
            enemyWaves.Clear();
            if (waves != null)
            {
                enemyWaves.AddRange(waves);
            }
        }
    }
