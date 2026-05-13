using UnityEngine;

[CreateAssetMenu(fileName = "Shop Room", menuName = "Room Config/Shop Room")]
public class ShopRoomConfigSO : RoomConfigSO
{
    [Header("Shop Settings")]
    [SerializeField] private GameObject merchantPrefab;
    [SerializeField] private int minItems = 3;
    [SerializeField] private int maxItems = 5;

    public GameObject MerchantPrefab => merchantPrefab;
    public int MinItems => minItems;
    public int MaxItems => maxItems;
}
