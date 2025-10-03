using System.Collections;
using UnityEngine;

public class HomingProjectile : Projectile
{
    [SerializeField] float rotateSpeed;
    GameObject target;
    bool hasHit;

    private void OnEnable()
    {
        hasHit = false;
    }
    private void Update()
    {
        HomeTowards(target, rotateSpeed);
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
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

