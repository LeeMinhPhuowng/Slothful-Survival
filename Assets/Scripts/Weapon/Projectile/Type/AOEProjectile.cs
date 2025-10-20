using System.Collections;
using UnityEngine;

public class AOEProjectile : Projectile
{
    [SerializeField] float radius;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        var enemies = Physics2D.OverlapCircleAll(collision.gameObject.transform.position, radius);
        foreach (var enemy in enemies)
        {
            Enemy target = enemy.gameObject.GetComponent<Enemy>();
            target.TakeDamage(damage);
        }
    }
}
