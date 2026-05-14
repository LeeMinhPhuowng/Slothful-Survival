using System.Collections;
using UnityEngine;

public class Twilight : AWeaponBehaviour
{
    [SerializeField] int projectileAmount;
    [SerializeField] WeaponInfoSO info;
    [SerializeField] float timeBetweenProjectiles;

    bool isFiring = false;

    public override bool Attack(Transform castPosition)
    {
        if(isFiring) { return false; }
        if (PlayerInfo.instance == null) return false;

        // Check for enemies before firing
        var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;

        StartCoroutine(Fire());
        return true;
    }

    IEnumerator Fire()
    {
        isFiring = true;

        for(int i = 0; i < projectileAmount; i++)
        {
            if (PlayerInfo.instance == null) break;

            var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
            if (enemies == null || enemies.Length == 0)
            {
                break;
            }
            Collider2D enemy = enemies[Random.Range(0, enemies.Length)];
            if(enemy != null)
            {
                int tmp = Random.Range(0, 2);
                if(tmp == 0)
                {
                    var projectile = ObjectPool.instance.SpawnFromPool(ObjectType.Dark, PlayerInfo.instance.transform.position, Quaternion.identity, (o) => 
                    { 
                        Projectile p = o.GetComponent<Projectile>();
                        if (p != null) p.Init(info.attackDamage, info.attackCooldown); 
                    });
                    
                    if (projectile != null)
                    {
                        HomingProjectile hp = projectile.GetComponent<HomingProjectile>();
                        if (hp != null) hp.SetTarget(enemy.gameObject);
                    }
                    yield return new WaitForSeconds(timeBetweenProjectiles);
                } 
                else
                {
                    var projectile = ObjectPool.instance.SpawnFromPool(ObjectType.Light, PlayerInfo.instance.transform.position, Quaternion.identity, (o) => 
                    { 
                        Projectile p = o.GetComponent<Projectile>();
                        if (p != null) p.Init(info.attackDamage, info.attackCooldown); 
                    });

                    if (projectile != null)
                    {
                        HomingProjectile hp = projectile.GetComponent<HomingProjectile>();
                        if (hp != null) hp.SetTarget(enemy.gameObject);
                    }
                    yield return new WaitForSeconds(timeBetweenProjectiles);
                }                
            }    
        }
        isFiring = false;
    }
}
