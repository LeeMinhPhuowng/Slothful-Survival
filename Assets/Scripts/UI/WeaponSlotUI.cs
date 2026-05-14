using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay; // The Image with Type = Filled

    private Weapon _boundWeapon;

    public void Bind(Weapon weapon)
    {
        _boundWeapon = weapon;
        if (weapon != null && weapon.Info != null)
        {
            iconImage.sprite = weapon.Info.icon;
            iconImage.enabled = (iconImage.sprite != null);
        }
        else
        {
            iconImage.enabled = false;
        }
    }

    private void Update()
    {
        if (_boundWeapon == null)
        {
            cooldownOverlay.fillAmount = 0;
            return;
        }

        // CooldownNormalized is 1 when just fired, 0 when ready
        cooldownOverlay.fillAmount = _boundWeapon.CooldownNormalized;
    }
}
