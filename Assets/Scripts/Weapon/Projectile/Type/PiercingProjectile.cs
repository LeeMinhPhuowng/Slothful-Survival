using System.Collections;
using UnityEngine;

public class PiercingProjectile : Projectile
{    
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
