using System.Collections;
using UnityEngine;

public class FallingStar : Projectile
{
    private Coroutine returnRoutine;
    public void OnActivate(int damage, int lifetime)
    {
        Init(damage, lifetime);
        MoveDownward();
       
        if(returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }
        returnRoutine = StartCoroutine(ReturnToPool());
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Debug.Log("OK");
            enemy.ReceiveDamage(damage);
            if(returnRoutine != null) { 
                StopCoroutine (returnRoutine);
                returnRoutine = null;
            }
            ObjectPool.instance.BackToPool(this.gameObject, ObjectType.ShootingStar);
        }
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPool.instance.BackToPool(this.gameObject, ObjectType.ShootingStar);
        returnRoutine = null;   
    }    
}
