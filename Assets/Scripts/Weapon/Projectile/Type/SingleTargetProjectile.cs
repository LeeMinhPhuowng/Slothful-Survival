using System.Collections;
using UnityEngine;

public class SingleTargetProjectile : Projectile
{
    bool hasHit;
    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            hasHit = true;
            enemyHealth.TakeDamage(damage);
            BackToPoolImmediately();
        }
    } 
}
