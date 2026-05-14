using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon Info", menuName = "Weapon/New Info")]
public class WeaponInfoSO : ScriptableObject
{
    public float attackDamage;
    public float attackCooldown; // Changed to float for precision
    public float attackRange;
    public WeaponID ID;
    public Sprite icon; // New field for UI
    public List<AugmentInfoSO> nextAugmentInfo;
}

