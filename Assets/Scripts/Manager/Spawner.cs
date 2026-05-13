    using System.Collections;
    using System.Collections.Generic;
using Game.UI.Data;
using UnityEngine;
    using UnityEngine.Tilemaps;

    public class Spawner : MonoBehaviour
    {
        public static Spawner Instance;
        
        [Header("Enemy Waves")]
        [SerializeField] List<EnemyWaveSO> enemyWaves;
        [SerializeField] Transform mapContainer;
        [SerializeField] Tilemap levelTilemap;
        LevelSO levelSO;
        List<Vector3> spawnPositions = new List<Vector3>();

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            levelSO = GameplayLaunchContext.MapConfig;
            // levelTilemap = GameObject.FindWithTag("MainTilemap").GetComponent<Tilemap>();
            Instantiate(levelSO.tilemapPrefab, mapContainer);
            InitializeEnemyWaves(levelSO.enemyWaves);
            InitializeSpawnPositions();
            StartCoroutine(SpawnEnemyWaves());
        }

        IEnumerator SpawnEnemyWaves()
        {
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
        }
            
        void InitializeSpawnPositions()
        {
            for (int x = levelSO.mapMinX; x <= levelSO.mapMaxX; x++)
            {
                for (int y = levelSO.mapMinY; y <= levelSO.mapMaxY; y++)
                {
                    if (x == levelSO.mapMinX || x == levelSO.mapMaxX || y == levelSO.mapMinY || y == levelSO.mapMaxY)
                    {
                        Vector3Int cellPos = new Vector3Int(x, y, 0);
                        Vector3 worldPos = levelTilemap.CellToWorld(cellPos) + levelTilemap.cellSize / 2f;
                        spawnPositions.Add(worldPos);
                    }
                }
            }
        }

        Vector3 GetSpawnPosition()
        {
            int index = Random.Range(0, spawnPositions.Count);
            return spawnPositions[index];
        }

        public void InitializeEnemyWaves(List<EnemyWaveSO> waves)
        {
            enemyWaves.AddRange(waves);
        }

    }
