using UnityEngine;

[CreateAssetMenu(fileName = "Boss Room", menuName = "Room Config/Boss Room")]
public class BossRoomConfigSO : RoomConfigSO
{
    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private AudioClip bossMusic;
    
    public GameObject BossPrefab => bossPrefab;
    public AudioClip BossMusic => bossMusic;
}
