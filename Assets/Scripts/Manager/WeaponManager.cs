using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public Transform[] weaponPositions = new Transform[3];
    public List<GameObject> weapons = new List<GameObject>();
    bool[] slotUsed = new bool[3];

    private void Start()
    {
    }

    public Transform GetWeaponSlot()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!slotUsed[i])
            {
                slotUsed[i] = true;
                return weaponPositions[i];
            }
        }
        return null;
    }

    public bool HasFreeSlot()
    {
        for (int i = 0; i < slotUsed.Length; i++)
        {
            if (!slotUsed[i]) return true;
        }
        return false;
    }    

    public void AddWeapon(GameObject weapon)
    {
        Transform parent = GetWeaponSlot();
        if (parent != null)
        {
            weapons.Add(Instantiate(weapon, parent));
        }
    }
}
