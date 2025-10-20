using System.Collections;
using UnityEngine;

public class BaseSword : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] float VFXExistTime = 0.35f;

    public override void Attack(Transform castPosition)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(castPosition.position, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return;
        float minDistance = Mathf.Infinity;
        Collider2D target = null;
        foreach(var enemy in enemies)
        {
            if(Vector2.Distance(castPosition.position, enemy.transform.position) < minDistance)
            {
                minDistance = Vector2.Distance(castPosition.position, enemy.transform.position);
                target = enemy;
            }
        }
        if(target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            GameObject vfx = ObjectPool.instance.SpawnFromPool(ObjectType.SlashVFX, target.transform.position);
            enemy.TakeDamage(info.attackDamage);
            StartCoroutine(ReturnVFX(vfx));
        }       
    }

    IEnumerator ReturnVFX(GameObject vfx)
    {
        yield return new WaitForSeconds(VFXExistTime);
        ObjectPool.instance.BackToPool(vfx, ObjectType.SlashVFX);
    }
}



