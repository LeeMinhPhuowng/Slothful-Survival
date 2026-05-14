using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponInfoSO info;
    public WeaponInfoSO Info => info;

    [SerializeField] AWeaponBehaviour behaviour;
    float coolDownLeft; 

    /// <summary>Returns 0 to 1 value for UI fill (1 = on cooldown, 0 = ready)</summary>
    public float CooldownNormalized => Mathf.Clamp01(coolDownLeft / info.attackCooldown);

    private void Start()
    {
        coolDownLeft = 0f;
    }
    private void Update()
    {
        if (coolDownLeft > 0)
        {
            coolDownLeft -= Time.deltaTime;
        }

        if(coolDownLeft <= 0)
        {
            if (behaviour.Attack(transform))
            {
                coolDownLeft = info.attackCooldown;
            }
        }
    }

}
