using System.Collections;
using UnityEngine;

public class Bloodmoon : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] float VFXExistTime;
    [SerializeField] int enemyAmount;
    [SerializeField] float timeBetweenSlashes;

    bool isSlashing = false;
    public override void Attack(Transform castPosition)
    {
        if(isSlashing) { return; }
        StartCoroutine(Slash());
    }

    IEnumerator Slash()
    {
        isSlashing = true;
        
        for(int i = 0; i < enemyAmount; i++)
        {
            var enemies = Physics2D.OverlapCircleAll(this.gameObject.transform.parent.position, info.attackRange, enemyLayer);
            if (enemies.Length == 0)
            {
                isSlashing = false;
                yield break;
            }
            Collider2D target = enemies[Random.Range(0, enemies.Length)];
            if(target != null)
            {
                Enemy enemy = target.GetComponent<Enemy>();
                GameObject vfx = ObjectPool.instance.SpawnFromPool(ObjectType.BloodSlashVFX, target.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
                //enemy?.TakeDamage(info.attackDamage);
                StartCoroutine(ReturnVFX(vfx));
                yield return new WaitForSeconds(timeBetweenSlashes);
            }
        }
        isSlashing = false;
    }

    IEnumerator ReturnVFX(GameObject vfx)
    {
        yield return new WaitForSeconds(VFXExistTime);
        ObjectPool.instance.BackToPool(vfx, ObjectType.BloodSlashVFX);
    }
}
