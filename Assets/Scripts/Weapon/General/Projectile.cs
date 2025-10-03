using System.Collections;
using System.Runtime.Versioning;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int lifetime;
    public ObjectType type;
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody2D rb;

    Coroutine returnRoutine;
    public void Init(int value, int value2)
    {
        damage = value;
        lifetime = value2;
        if(returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }    
    }
    
    protected void OnEnable()
    {
        returnRoutine = StartCoroutine(ReturnToPool());
    }

    protected void OnDisable()
    {
        if(returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }    
    }

    /* Movement method */
    public void MoveForward()
    {
        rb.linearVelocity = transform.right * moveSpeed;
    }

    public void MoveDownward()
    {
        rb.linearVelocity = -transform.up * moveSpeed;
    }

    public void HomeTowards(GameObject target, float rotateSpeed)
    {
        if (target == null) return;
        Vector2 direction = (target.transform.position - gameObject.transform.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.right).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.linearVelocity = transform.right * moveSpeed;
    }

    /* Return method (After lifetime by default) */
    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(lifetime);
        BackToPoolImmediately();
    }

    protected void BackToPoolImmediately()
    {
        if(returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }
        ObjectPool.instance.BackToPool(this.gameObject, type);
    }

}
