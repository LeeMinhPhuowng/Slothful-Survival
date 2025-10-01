using System.Collections;
using UnityEngine;

public class PiercingProjectile : Projectile
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Debug.Log("OK");
            enemy.ReceiveDamage(damage);
            StartCoroutine(ReturnToPool());
        }
    }
    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPool.instance.BackToPool(this.gameObject, type);
    }
}
