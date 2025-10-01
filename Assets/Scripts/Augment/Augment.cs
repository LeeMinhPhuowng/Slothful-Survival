using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Augment : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image itemImage;
    [SerializeField] Image augmentImage;
    private AugmentInfoSO augmentInfo;

    public void Init(AugmentInfoSO info)
    {
        nameText.text = info.name;
        description.text = info.description;
        itemImage.sprite = info.image;
        augmentImage.sprite = info.frame;
        augmentInfo = info;
    }

    private void ApplyAugment()
    {
        switch (augmentInfo.augmentType)
        {
            case AugmentType.Buff:
                BuffAugment((BuffInfoSO)augmentInfo);
                break;
            case AugmentType.Add:
                AddAugment((AddInfoSO)augmentInfo);
                break;
            case AugmentType.Upgrade:
                UpgradeAugment((UpgradeInfoSO)augmentInfo);
                break;
        }
    }


    private void BuffAugment(BuffInfoSO info)
    {
        switch (info.Type)
        {
            case StatType.Health:
                PlayerInfo.instance.MaxHealth += info.amount;
                break;
            case StatType.Healing:
                float healAmount = PlayerInfo.instance.MaxHealth * 0.25f;
                PlayerInfo.instance.CurrentHealth += healAmount;
                break;
            case StatType.MoveSpeed:
                PlayerInfo.instance.MoveSpeed += info.amount;
                break;
            case StatType.PickupRange:
                PlayerInfo.instance.PickupRange += info.amount;
                break;
            default:
                return;
        }
    }    

    private void AddAugment(AddInfoSO info)
    {
        WeaponManager.Instance.AddWeapon(info.NewWeapon);
        AugmentManager.Instance.addInfos.Remove(info);
    }

    private void UpgradeAugment(UpgradeInfoSO info)
    {
        for (int i = 0; i < WeaponManager.Instance.weapons.Count; i++)
        {
            GameObject weapon = WeaponManager.Instance.weapons[i];
            Weapon weaponComponent = weapon.GetComponent<Weapon>();
            if(info.SourceWeaponID ==  weaponComponent.Info.ID)
            {
                Transform parent = WeaponManager.Instance.weaponPositions[i];
                GameObject upgradedWeapon = Instantiate(info.UpgradedWeapon, parent);
                WeaponManager.Instance.weapons[i] = upgradedWeapon;
                Destroy(weapon);
                break;
            }
        }
    }

    public void OnClick()
    {
        ApplyAugment();
    }
}