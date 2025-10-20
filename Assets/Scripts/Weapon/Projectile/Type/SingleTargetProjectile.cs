using System.Collections;
using UnityEngine;

public class SingleTargetProjectile : Projectile
{
    bool hasHit;
    private void OnEnable()
    {
        hasHit = false;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage);
            BackToPoolImmediately();
        }
    } 
}
