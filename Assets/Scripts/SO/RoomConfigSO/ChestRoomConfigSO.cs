using UnityEngine;

[CreateAssetMenu(fileName = "Chest Room", menuName = "Room Config/Chest Room")]
public class ChestRoomConfigSO : RoomConfigSO
{
    [Header("Chest Settings")]
    [SerializeField] private GameObject chestPrefab;

    public GameObject ChestPrefab => chestPrefab;
}
