using System.Collections;
using UnityEngine;

public class BaseSword : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] float VFXExistTime = 0.35f;

    public override bool Attack(Transform castPosition)
    {
        if (PlayerInfo.instance == null) return false;
        Vector3 playerPos = PlayerInfo.instance.transform.position;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(playerPos, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;
        
        float minDistance = Mathf.Infinity;
        Collider2D target = null;
        foreach(var enemy in enemies)
        {
            float dist = Vector2.Distance(playerPos, enemy.transform.position);
            if(dist < minDistance)
            {
                minDistance = dist;
                target = enemy;
            }
        }
        if(target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            GameObject vfx = ObjectPool.instance.SpawnFromPool(ObjectType.SlashVFX, target.transform.position);
            enemy?.TakeDamage(info.attackDamage);
            StartCoroutine(ReturnVFX(vfx));
            return true;
        }       
        return false;
    }

    IEnumerator ReturnVFX(GameObject vfx)
    {
        yield return new WaitForSeconds(VFXExistTime);
        ObjectPool.instance.BackToPool(vfx, ObjectType.SlashVFX);
    }
}



