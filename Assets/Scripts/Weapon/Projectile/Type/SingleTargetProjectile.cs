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
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.ReceiveDamage(damage);
            hasHit = true;
            ObjectPool.instance.BackToPool(this.gameObject, type);
        }
    }
}
