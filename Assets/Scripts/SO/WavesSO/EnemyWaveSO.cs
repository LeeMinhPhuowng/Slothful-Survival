using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "New Enemy Wave", menuName = "Enemy/New Wave")]
public class EnemyWaveSO : ScriptableObject
{
    public List<GameObject> enemies = new List<GameObject>();
    public float timeTillNextWave;
}
