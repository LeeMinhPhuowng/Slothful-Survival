using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponInfoSO info;
    public WeaponInfoSO Info => info;

    [SerializeField] AWeaponBehaviour behaviour;
    float coolDownLeft; 
    private void Start()
    {
        coolDownLeft = 0f;
    }
    private void Update()
    {
        coolDownLeft -= Time.deltaTime;
        if(coolDownLeft <= 0)
        {
            Debug.Log("Attack Called");
            behaviour.Attack(transform);
            coolDownLeft = info.attackCooldown;
        }
    }

}
