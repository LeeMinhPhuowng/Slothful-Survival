using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "New Level")]

public class LevelSO : ScriptableObject
{
    public string levelName;
    public GameObject tilemapPrefab;
    public List<EnemyWaveSO> enemyWaves = new List<EnemyWaveSO>();
    public float levelTime;
    public int mapMaxX;
    public int mapMaxY;
    public int mapMinX;
    public int mapMinY;
}
