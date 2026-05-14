using System.Collections;
using UnityEngine;

public class Devastator : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] int projectileAmount = 5;
    [SerializeField] float timeBetweenFires = 0.5f;

    private bool isFiring = false;

    public override bool Attack(Transform castPosition)
    {
        if (isFiring) return false;
        if (PlayerInfo.instance == null) return false;

        // Check if there are enemies in range to trigger the attack
        var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;

        StartCoroutine(FireRandomBurst());
        return true;
    }

    private IEnumerator FireRandomBurst()
    {
        isFiring = true;
        if (projectilePrefab == null) { Debug.LogError("Devastator: Missing Projectile Prefab!"); isFiring = false; yield break; }
        if (ObjectPool.instance == null) { Debug.LogError("Devastator: ObjectPool instance is null!"); isFiring = false; yield break; }
        
        Projectile prefProj = projectilePrefab.GetComponent<Projectile>();
        if (prefProj == null) { Debug.LogError("Devastator: Prefab missing Projectile component!"); isFiring = false; yield break; }

        ObjectType type = prefProj.type;
        Debug.Log($"Devastator: Starting burst of {projectileAmount} shots.");

        for (int i = 0; i < projectileAmount; i++)
        {
            if (PlayerInfo.instance == null) break;

            // Random direction in 360 degrees
            float randomAngle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);
            Vector3 playerPos = PlayerInfo.instance.transform.position;

            var projectileObj = ObjectPool.instance.SpawnFromPool(type, playerPos, rotation, (o) => 
            { 
                Projectile p = o.GetComponent<Projectile>();
                if (p != null)
                {
                    p.Init(info.attackDamage, info.attackCooldown); 
                    p.enemyLayer = enemyLayer;
                }
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
