using System.Collections;
using UnityEngine;

public class AOEProjectile : Projectile
{
    [SerializeField] float radius;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemies = Physics2D.OverlapCircleAll(collision.gameObject.transform.position, radius);
        foreach (var enemy in enemies)
        {
            EnemyHealth enemyHealth = enemy.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damage);
        }
    }
}
