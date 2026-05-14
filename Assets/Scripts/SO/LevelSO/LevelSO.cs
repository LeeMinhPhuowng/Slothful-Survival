using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "New Level")]

public class LevelSO : ScriptableObject
{
    public string levelName;
    public MapConfigSO mapConfig;
    public List<EnemyWaveSO> enemyWaves = new List<EnemyWaveSO>();
    public float levelTime;
}
