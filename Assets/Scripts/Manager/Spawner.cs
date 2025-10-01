    using System.Collections;
    using System.Collections.Generic;
        using UnityEngine;
        using UnityEngine.Tilemaps;

        public class Spawner : MonoBehaviour
        {
            public static Spawner Instance;
        
            [Header("Tilemap Properties")]
            [SerializeField] Tilemap grassTilemap;
            [SerializeField] int xMax;
            [SerializeField] int yMax;
            [SerializeField] int xMin;
            [SerializeField] int yMin;
            [Header("Enemy Waves")]
            [SerializeField] List<EnemyWaveSO> enemyWaves;
    
            List<Vector3> spawnPositions = new List<Vector3>();

        private void Awake()
        {
            Instance = this;
        }

        void Start()
            {
                InitializeSpawnPositions();
                StartCoroutine(SpawnEnemyWaves());
            }

            IEnumerator SpawnEnemyWaves()
            {
                foreach (var wave in enemyWaves)
                {
                    foreach (var enemy in wave.enemies)
                    {
                        Enemy target = enemy.GetComponent<Enemy>();
                        Vector3 spawnPos = GetSpawnPosition();
                        ObjectPool.instance.SpawnFromPool(target.info.type, spawnPos);
                    }
                    yield return new WaitForSeconds(wave.timeTillNextWave);
                }
            }

            void InitializeSpawnPositions()
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        if (x == xMin || x == xMax || y == yMin || y == yMax)
                        {
                            Vector3Int cellPos = new Vector3Int(x, y, 0);
                            Vector3 worldPos = grassTilemap.CellToWorld(cellPos) + grassTilemap.cellSize / 2f;
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
        }
