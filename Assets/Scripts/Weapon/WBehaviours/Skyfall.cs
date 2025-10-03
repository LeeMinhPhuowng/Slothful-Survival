using System.Collections;
using TMPro;
using UnityEngine;

public class Skyfall : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] Vector3 spawnPos;
    [SerializeField] int amount;
    [SerializeField] float timeBetween;

    bool isSummoning = false;
    public override void Attack(Transform castPosition)
    {
        if (isSummoning) { return; }
        StartCoroutine(SummonStars());
    }

    IEnumerator SummonStars()
    {
        isSummoning = true;
        var enemies = Physics2D.OverlapCircleAll(this.gameObject.transform.parent.position, info.attackRange, enemyLayer);
        for (int i = 0; i < amount; ++i)
        {
            Collider2D target = enemies[Random.Range(0, enemies.Length)];
            if(target != null)
            {
                var shootingStar = ObjectPool.instance.SpawnFromPool(ObjectType.ShootingStar, target.gameObject.transform.position + spawnPos, Quaternion.identity, (o) => { o.GetComponent<Projectile>().Init(info.attackDamage, info.attackCooldown); });
                shootingStar.GetComponent<Projectile>().MoveDownward();
                yield return new WaitForSeconds(timeBetween);
            }
        }
        isSummoning = false;   
    }    
}
