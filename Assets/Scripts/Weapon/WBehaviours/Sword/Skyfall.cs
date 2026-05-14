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
    public override bool Attack(Transform castPosition)
    {
        if (isSummoning) { return false; }
        if (PlayerInfo.instance == null) return false;

        // Check for enemies before starting the summoning process
        var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;

        StartCoroutine(SummonStars());
        return true;
    }

    IEnumerator SummonStars()
    {
        isSummoning = true;
        
        if (PlayerInfo.instance == null) { isSummoning = false; yield break; }

        // Re-scan to get fresh targets
        var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
        if (enemies == null || enemies.Length == 0)
        {
            isSummoning = false;
            yield break;
        }

        for (int i = 0; i < amount; ++i)
        {
            Collider2D target = enemies[Random.Range(0, enemies.Length)];
            if (target != null)
            {
                var shootingStar = ObjectPool.instance.SpawnFromPool(ObjectType.ShootingStar, target.gameObject.transform.position + spawnPos, Quaternion.identity, (o) => 
                { 
                    Projectile p = o.GetComponent<Projectile>();
                    p.Init(info.attackDamage, info.attackCooldown); 
                    p.enemyLayer = enemyLayer;
                });
                shootingStar.GetComponent<Projectile>().MoveDownward();
                yield return new WaitForSeconds(timeBetween);
            }
        }
        isSummoning = false;
    }
}
