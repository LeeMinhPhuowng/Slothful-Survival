using UnityEngine;

public class BaseBow : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject projectilePrefab;

    public override bool Attack(Transform castPosition)
    {
        if (PlayerInfo.instance == null || info == null || ObjectPool.instance == null) return false;
        
        Vector3 playerPos = PlayerInfo.instance.transform.position;

        // Find enemies in range
        var enemies = Physics2D.OverlapCircleAll(playerPos, info.attackRange, enemyLayer);
        if (enemies == null || enemies.Length == 0) return false;
        
        float minDistance = Mathf.Infinity;
        Collider2D target = null;
        
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            float dist = Vector2.Distance(playerPos, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                target = enemy;
            }
        }

        if (target != null)
        {
            Vector2 direction = (target.transform.position - playerPos).normalized;
            Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, direction);
            
            // Safe check for prefab
            if (projectilePrefab == null) 
            {
                Debug.LogWarning($"[BaseBow] Missing projectilePrefab on {gameObject.name}");
                return false;
            }

            Projectile prefProj = projectilePrefab.GetComponent<Projectile>();
            if (prefProj == null) 
            {
                Debug.LogWarning($"[BaseBow] Projectile component missing on prefab for {gameObject.name}");
                return false;
            }

            // Spawn from pool using the type defined in the prefab
            var projectileObj = ObjectPool.instance.SpawnFromPool(prefProj.type, playerPos, quaternion, (o) => 
            { 
                Projectile p = o.GetComponent<Projectile>();
                if (p != null) p.Init(info.attackDamage, info.attackCooldown); 
            });

            if (projectileObj != null)
            {
                Projectile pComp = projectileObj.GetComponent<Projectile>();
                if (pComp != null) pComp.MoveForward();
                return true;
            }
        }
        return false;
    }
}
