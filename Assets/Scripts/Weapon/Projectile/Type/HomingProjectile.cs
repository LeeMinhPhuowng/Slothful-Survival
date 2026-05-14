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
        // Safety check: if target is destroyed or inactive, just fly forward
        if (target != null && target.activeInHierarchy)
        {
            HomeTowards(target, rotateSpeed);
        }
        else
        {
            // If target is lost, just move straight
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
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

