using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    
    [Header("Settings")]
    [SerializeField] private int maxSlots = 6;
    
    // The master list of weapons the player currently has
    private List<Weapon> activeWeapons = new List<Weapon>();

    public IReadOnlyList<Weapon> ActiveWeapons => activeWeapons;
    public int MaxSlots => maxSlots;

    private void Awake()
    {
        Instance = this;
    }

    public bool HasFreeSlot()
    {
        return activeWeapons.Count < maxSlots;
    }

    public void AddWeapon(GameObject weaponPrefab)
    {
        if (!HasFreeSlot())
        {
            Debug.LogWarning("[WeaponManager] No free slots available!");
            return;
        }

        // Instantiate the weapon as a child of the player (WeaponManager is usually on Player)
        GameObject weaponGo = Instantiate(weaponPrefab, transform);
        Weapon weapon = weaponGo.GetComponent<Weapon>();
        
        if (weapon != null)
        {
            activeWeapons.Add(weapon);
            Debug.Log($"[WeaponManager] Added weapon: {weapon.Info.ID}");
        }
    }

    /// <summary>
    /// Replaces an existing weapon with a new one (used for Upgrades)
    /// </summary>
    public void ReplaceWeapon(WeaponID oldWeaponID, GameObject newWeaponPrefab)
    {
        for (int i = 0; i < activeWeapons.Count; i++)
        {
            if (activeWeapons[i].Info.ID == oldWeaponID)
            {
                Weapon oldWeapon = activeWeapons[i];
                
                // Instantiate new one
                GameObject newWeaponGo = Instantiate(newWeaponPrefab, transform);
                Weapon newWeapon = newWeaponGo.GetComponent<Weapon>();
                
                // Swap in list
                activeWeapons[i] = newWeapon;
                
                // Cleanup old
                Destroy(oldWeapon.gameObject);
                
                Debug.Log($"[WeaponManager] Upgraded {oldWeaponID} to {newWeapon.Info.ID}");
                return;
            }
        }
    }
}
