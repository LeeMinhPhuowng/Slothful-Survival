using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon Info", menuName = "Weapon/New Info")]
public class WeaponInfoSO : ScriptableObject
{
    public int attackDamage;
    public int attackCooldown;
    public int attackRange;
    public WeaponID ID;
    public List<AugmentInfoSO> nextAugmentInfo;
}

