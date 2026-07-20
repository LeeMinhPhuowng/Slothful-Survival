using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Connects the Grid Inventory system with the real-time Gameplay WeaponManager.
    /// Manages two inventories:
    /// 1. Big Inventory (10x10 Stash) - items placed here do not spawn weapons, bag requirement is bypassed.
    /// 2. Small Inventory (starts 2x2) - items placed here are spawned on the player and auto-attack.
    /// Does NOT use EventBus or Reflex DI. Uses standard C# events.
    /// </summary>
    public class GameplayInventoryBridge : MonoBehaviour
    {
        public static GameplayInventoryBridge Instance { get; private set; }

        [System.Serializable]
        public struct ItemWeaponMapping
        {
            [Tooltip("Name of the ItemSO asset, e.g. Sword, Slot1x1")]
            public string itemSOName;
            [Tooltip("The actual weapon prefab to spawn on the player")]
            public GameObject weaponPrefab;
        }

        [Header("UI Toggle Settings")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private KeyCode toggleKey = KeyCode.I;

        [Header("Inventory Views")]
        [SerializeField] private InventoryView bigInventoryView;   // 10x10 Stash
        [SerializeField] private InventoryView smallInventoryView; // 2x2 Active Bag

        [Header("Weapon Mappings")]
        [SerializeField] private List<ItemWeaponMapping> weaponMappings;

        [Header("Starting Items Configuration")]
        [Tooltip("The 2x2 Bag ItemSO that will be automatically placed in the Active Bag at Start to unlock the grid")]
        [SerializeField] private ItemSO startingBagItem;

        private readonly Dictionary<ItemModel, Weapon> _spawnedWeapons = new Dictionary<ItemModel, Weapon>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Configure Big Inventory to bypass bag requirement so players can put items anywhere
            if (bigInventoryView != null && bigInventoryView.Model != null)
            {
                bigInventoryView.Model.BypassBagRequirement = true;
            }

            // Configure Small Inventory to strictly enforce bag requirement (starting bag cells)
            EnsureSmallInventoryInitialized();

            // Set default active state for UI panel
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }
        }

        private void Update()
        {
            // Toggle inventory visibility
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleInventory();
            }
        }

        public void ToggleInventory()
        {
            if (inventoryPanel != null)
            {
                bool isActive = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(isActive);
                
                // Optional: Pause gameplay or unlock cursor when inventory is open
                if (isActive)
                {
                    Time.timeScale = 0f; // Pause game
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Time.timeScale = 1f; // Resume game
                    // Restore cursor state as appropriate for your game
                }
            }
        }

        /// <summary>
        /// Maps a weapon prefab back to its corresponding ItemSO from the inventory config
        /// and attempts to place it in the Big Inventory (Stash).
        /// </summary>
        public bool AddWeaponToStash(GameObject weaponPrefab)
        {
            if (bigInventoryView == null || bigInventoryView.Model == null)
            {
                Debug.LogError("[GameplayInventoryBridge] Big Inventory View is not assigned or initialized!");
                return false;
            }

            // 1. Find the ItemSO Name mapped to this weapon prefab
            string itemSOName = FindItemSOName(weaponPrefab);
            if (string.IsNullOrEmpty(itemSOName))
            {
                Debug.LogError($"[GameplayInventoryBridge] No mapped ItemSO name found for weapon prefab: {weaponPrefab.name}");
                return false;
            }

            // Đảm bảo cờ BypassBagRequirement được bật TRƯỚC khi tìm vị trí đặt vũ khí khởi đầu!
            // (Tránh lỗi thứ tự chạy Start() của các class trong Unity)
            bigInventoryView.Model.BypassBagRequirement = true;

            // 2. Resolve the ItemSO reference from the Big Inventory's config
            ItemSO itemSO = bigInventoryView.Model.GetConfig()?.GetItemSOByName(itemSOName);
            if (itemSO == null)
            {
                Debug.LogError($"[GameplayInventoryBridge] ItemSO '{itemSOName}' is not registered in the InventoryConfigSO array!");
                return false;
            }

            // 3. Find a free slot in the Big Inventory that can fit this itemSO
            if (FindFreePosition(bigInventoryView.Model, itemSO, out Vector2Int freePos, out ItemSO.Dir freeDir))
            {
                bool placed = bigInventoryView.Model.TryPlaceItem(itemSO, freePos, freeDir);
                if (placed)
                {
                    Debug.Log($"[GameplayInventoryBridge] Successfully added {itemSOName} to Stash at {freePos} ({freeDir})");
                    return true;
                }
            }

            Debug.LogWarning($"[GameplayInventoryBridge] Stash is full! Cannot fit item {itemSOName}");
            return false;
        }

        private bool _isSmallInventoryInitialized = false;

        private void EnsureSmallInventoryInitialized()
        {
            if (_isSmallInventoryInitialized) return;

            if (smallInventoryView != null && smallInventoryView.Model != null)
            {
                smallInventoryView.Model.BypassBagRequirement = false;
                // Subscribe events safely
                smallInventoryView.Model.OnItemPlaced -= OnItemPlaced;
                smallInventoryView.Model.OnItemPlaced += OnItemPlaced;
                smallInventoryView.Model.OnItemRemoved -= OnItemRemoved;
                smallInventoryView.Model.OnItemRemoved += OnItemRemoved;

                // Tự động đặt túi lót 2x2 khởi đầu vào Active Bag ở vị trí (0, 0)
                if (startingBagItem != null)
                {
                    bool bagPlaced = smallInventoryView.Model.TryPlaceItem(startingBagItem, Vector2Int.zero, ItemSO.Dir.Down);
                    if (bagPlaced)
                    {
                        Debug.Log("[GameplayInventoryBridge] Placed starting 2x2 Bag in Active Bag grid.");
                    }
                    else
                    {
                        Debug.LogError("[GameplayInventoryBridge] Failed to place starting 2x2 bag in Active Bag!");
                    }
                }
            }
            _isSmallInventoryInitialized = true;
        }

        /// <summary>
        /// Maps a weapon prefab back to its corresponding ItemSO from the inventory config
        /// and attempts to place it in the Small Inventory (Active Bag) so it equips automatically.
        /// </summary>
        public bool AddWeaponToActiveBag(GameObject weaponPrefab)
        {
            if (smallInventoryView == null || smallInventoryView.Model == null)
            {
                Debug.LogError("[GameplayInventoryBridge] Small Inventory View is not assigned or initialized!");
                return false;
            }

            string itemSOName = FindItemSOName(weaponPrefab);
            if (string.IsNullOrEmpty(itemSOName))
            {
                Debug.LogError($"[GameplayInventoryBridge] No mapped ItemSO name found for weapon prefab: {weaponPrefab.name}");
                return false;
            }

            // Ensure bag is placed first (safeguard for Script Execution Order)
            EnsureSmallInventoryInitialized();

            ItemSO itemSO = smallInventoryView.Model.GetConfig()?.GetItemSOByName(itemSOName);
            if (itemSO == null)
            {
                Debug.LogError($"[GameplayInventoryBridge] ItemSO '{itemSOName}' is not registered in the InventoryConfigSO array!");
                return false;
            }

            if (FindFreePosition(smallInventoryView.Model, itemSO, out Vector2Int freePos, out ItemSO.Dir freeDir))
            {
                bool placed = smallInventoryView.Model.TryPlaceItem(itemSO, freePos, freeDir);
                if (placed)
                {
                    Debug.Log($"[GameplayInventoryBridge] Successfully added {itemSOName} to Active Bag at {freePos} ({freeDir})");
                    return true;
                }
            }

            Debug.LogWarning($"[GameplayInventoryBridge] Active Bag is full! Cannot fit item {itemSOName}");
            return false;
        }

        private void OnItemPlaced(ItemModel item)
        {
            ItemSO itemSO = item.ItemSO;

            // Only care about Regular items (Weapons/Equipments)
            if (itemSO.itemType != ItemType.Regular) return;

            // Try to find the mapped gameplay weapon prefab
            GameObject weaponPrefab = FindWeaponPrefab(itemSO.name);
            if (weaponPrefab == null)
            {
                Debug.LogWarning($"[GameplayInventoryBridge] No gameplay weapon prefab mapping found for ItemSO: {itemSO.name}");
                return;
            }

            if (WeaponManager.Instance == null)
            {
                Debug.LogError("[GameplayInventoryBridge] WeaponManager.Instance is not initialized in the scene!");
                return;
            }

            // Spawn the weapon on the player via WeaponManager
            Weapon spawnedWeapon = WeaponManager.Instance.AddWeapon(weaponPrefab);
            if (spawnedWeapon != null)
            {
                _spawnedWeapons[item] = spawnedWeapon;
                Debug.Log($"[GameplayInventoryBridge] Spawned and tracked gameplay weapon for {itemSO.name} in Active Bag");
            }
        }

        private void OnItemRemoved(ItemModel item)
        {
            if (_spawnedWeapons.TryGetValue(item, out Weapon spawnedWeapon))
            {
                if (WeaponManager.Instance != null && spawnedWeapon != null)
                {
                    WeaponManager.Instance.RemoveWeapon(spawnedWeapon);
                }
                _spawnedWeapons.Remove(item);
                Debug.Log($"[GameplayInventoryBridge] Removed and cleaned up gameplay weapon for {item.ItemSO.name} from Active Bag");
            }
        }

        private GameObject FindWeaponPrefab(string itemSOName)
        {
            foreach (var mapping in weaponMappings)
            {
                // Compare with both itemSO Asset Name and nameString for tolerance
                if (mapping.itemSOName == itemSOName)
                {
                    return mapping.weaponPrefab;
                }
            }
            return null;
        }

        private string FindItemSOName(GameObject weaponPrefab)
        {
            foreach (var mapping in weaponMappings)
            {
                if (mapping.weaponPrefab == weaponPrefab)
                {
                    return mapping.itemSOName;
                }
                
                // Fallback check by name string comparison to tolerate duplicate references/instantiations
                if (mapping.weaponPrefab != null && mapping.weaponPrefab.name == weaponPrefab.name)
                {
                    return mapping.itemSOName;
                }
            }
            return null;
        }

        /// <summary>
        /// Scans the entire grid horizontally and vertically in all 4 rotations to find a free space.
        /// </summary>
        private bool FindFreePosition(InventoryModel model, ItemSO itemSO, out Vector2Int freePos, out ItemSO.Dir freeDir)
        {
            freePos = Vector2Int.zero;
            freeDir = ItemSO.Dir.Down;

            int width = model.GetGrid().GetWidth();
            int height = model.GetGrid().GetHeight();

            // Loop through all directions
            foreach (ItemSO.Dir dir in Enum.GetValues(typeof(ItemSO.Dir)))
            {
                // Loop through all grid coordinates
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Vector2Int candidate = new Vector2Int(x, y);
                        if (model.CanPlaceItem(itemSO, candidate, dir))
                        {
                            freePos = candidate;
                            freeDir = dir;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void OnDestroy()
        {
            if (smallInventoryView != null && smallInventoryView.Model != null)
            {
                smallInventoryView.Model.OnItemPlaced -= OnItemPlaced;
                smallInventoryView.Model.OnItemRemoved -= OnItemRemoved;
            }
            
            // In case scene is unloaded, reset timeScale
            Time.timeScale = 1f;
        }
    }
}
