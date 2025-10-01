using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New Upgrade Augment", menuName = "Augment/Upgrade")]
public class UpgradeInfoSO : AugmentInfoSO
{
    public WeaponID SourceWeaponID;
    public GameObject UpgradedWeapon;
}
