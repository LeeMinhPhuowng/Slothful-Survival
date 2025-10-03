using UnityEngine;

public class GuardiansBlade : MonoBehaviour
{
    [SerializeField] WeaponInfoSO info; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
        if(enemyHealth != null) enemyHealth.TakeDamage(info.attackDamage);
    }
}
