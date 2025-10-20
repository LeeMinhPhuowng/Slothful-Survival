using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
[CreateAssetMenu(fileName = "New Enemy Wave", menuName = "Enemy/New Wave")]
public class EnemyWaveSO : ScriptableObject
{
    [System.Serializable] 
    public class WaveInfo
    {
        public ObjectType type;
        public int amount;
    }

    public List<WaveInfo> waveInfos;
    public float timeTillNextWave;
}
