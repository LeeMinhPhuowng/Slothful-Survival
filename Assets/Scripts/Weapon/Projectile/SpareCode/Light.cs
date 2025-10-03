using System.Collections;
using UnityEngine;

public class Light : HomingProjectile
{
    private Coroutine returnRoutine;

    public void OnActivate(int damage, int lifetime, GameObject target)
    {
        Init(damage, lifetime);
        SetTarget(target);

        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }
        returnRoutine = StartCoroutine(ReturnToPool());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            Debug.Log("OK");
            enemyHealth.TakeDamage(damage);
            if (returnRoutine != null)
            {
                StopCoroutine(returnRoutine);
                returnRoutine = null;
            }
            ObjectPool.instance.BackToPool(this.gameObject, ObjectType.Light);
        }
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPool.instance.BackToPool(this.gameObject, ObjectType.Light);
        returnRoutine = null;
    }
}
