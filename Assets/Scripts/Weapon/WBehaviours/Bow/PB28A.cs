using System.Collections;
using UnityEngine;

public class PB28A : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Vector2[] directions;
    [SerializeField] float timeBetweenFires;

    bool isFiring = false;
    public override bool Attack(Transform castPosition)
    {
        if (isFiring) { return false; }
        if (PlayerInfo.instance == null) return false;
        
        // Laser can always fire or check for enemies
        var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;

        StartCoroutine(FireLasers());
        return true;
    }
    
    IEnumerator FireLasers()
    {
        isFiring = true;
        if (projectilePrefab == null || ObjectPool.instance == null) { isFiring = false; yield break; }
        Projectile prefProj = projectilePrefab.GetComponent<Projectile>();
        if (prefProj == null) { isFiring = false; yield break; }

        for(int i = 0; i < directions.Length; i++)
        {
            if (PlayerInfo.instance == null) break;

            Vector2 direction = (directions[i]).normalized;
            Quaternion rotation = Quaternion.FromToRotation(Vector2.right, direction);
            var projectileObj = ObjectPool.instance.SpawnFromPool(prefProj.type, PlayerInfo.instance.gameObject.transform.position + (Vector3)directions[i], rotation, (o) => 
            { 
                Projectile p = o.GetComponent<Projectile>();
                if (p != null) p.Init(info.attackDamage, info.attackCooldown); 
            });

            if (projectileObj != null)
            {
                Projectile pComp = projectileObj.GetComponent<Projectile>();
                if (pComp != null) pComp.MoveForward();
            }

            yield return new WaitForSeconds(timeBetweenFires);
        }
        isFiring = false;
    }
}
