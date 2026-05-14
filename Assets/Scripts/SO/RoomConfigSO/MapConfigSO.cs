using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Config", menuName = "Map Config")]
public class MapConfigSO : ScriptableObject
{
    [SerializeField] private int numberOfRooms = 10;
    [SerializeField] private float cellSize = 1.0f;
    [SerializeField] private float chestRoomRatio;
    [SerializeField] private float shopRoomRatio;
    
    [Header("Room Configurations")]
    [SerializeField] private List<DefaultRoomConfigSO> defaultRoomConfigs;
    [SerializeField] private List<ChestRoomConfigSO> chestRoomConfigs;
    [SerializeField] private List<ShopRoomConfigSO> shopRoomConfigs;
    [SerializeField] private List<BossRoomConfigSO> bossRoomConfigs;

    public int NumberOfRooms => numberOfRooms;
    public float CellSize => cellSize;
    public float ChestRoomRatio => chestRoomRatio;
    public float ShopRoomRatio => shopRoomRatio;    

    public List<DefaultRoomConfigSO> DefaultRoomConfigs => defaultRoomConfigs;
    public List<ChestRoomConfigSO> ChestRoomConfigs => chestRoomConfigs;
    public List<ShopRoomConfigSO> ShopRoomConfigs => shopRoomConfigs;
    public List<BossRoomConfigSO> BossRoomConfigs => bossRoomConfigs;
}


