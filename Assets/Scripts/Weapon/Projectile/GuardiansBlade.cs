using UnityEngine;

public class GuardiansBlade : MonoBehaviour
{
    [SerializeField] WeaponInfoSO info; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if(enemy != null) enemy.ReceiveDamage(info.attackDamage);
    }
}
