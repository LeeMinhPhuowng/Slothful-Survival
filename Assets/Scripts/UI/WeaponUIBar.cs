using System.Collections.Generic;
using UnityEngine;

public class WeaponUIBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform container; // Usually a VerticalLayoutGroup

    private List<WeaponSlotUI> _slots = new List<WeaponSlotUI>();

    private void Start()
    {
        // Pre-create all slots based on WeaponManager's max capacity
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        // Clear existing children in container just in case
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        int maxSlots = WeaponManager.Instance != null ? WeaponManager.Instance.MaxSlots : 6;

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject go = Instantiate(slotPrefab, container);
            WeaponSlotUI slot = go.GetComponent<WeaponSlotUI>();
            _slots.Add(slot);
            slot.Bind(null); // Initialize as empty
        }
    }

    private void Update()
    {
        if (WeaponManager.Instance == null) return;

        var activeWeapons = WeaponManager.Instance.ActiveWeapons;

        // Update each slot based on active weapons
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < activeWeapons.Count)
            {
                _slots[i].Bind(activeWeapons[i]);
            }
            else
            {
                _slots[i].Bind(null);
            }
        }
    }
}
