using UnityEngine;

public class BoomerangBehaviour : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject boomerangPrefab;
    [SerializeField] float travelDistance;

    public override bool Attack(Transform castPosition)
    {
        if (PlayerInfo.instance == null) return false;
        if (info == null)
        {
            Debug.LogError($"[BoomerangBehaviour] Missing WeaponInfoSO on {gameObject.name}!");
            return false;
        }

        Vector3 playerPos = PlayerInfo.instance.transform.position;

        // Check for enemies
        var enemies = Physics2D.OverlapCircleAll(playerPos, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;

        // Find closest enemy
        float minDistance = Mathf.Infinity;
        Collider2D target = null;
        foreach (var enemy in enemies)
        {
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
            Quaternion rotation = Quaternion.FromToRotation(Vector2.right, direction);
            
            if (boomerangPrefab == null)
            {
                Debug.LogError($"[BoomerangBehaviour] Missing BoomerangPrefab on {gameObject.name}!");
                return false;
            }

            Projectile prefProj = boomerangPrefab.GetComponent<Projectile>();
            if (prefProj == null) return false;

            if (ObjectPool.instance == null) return false;

            var projectileObj = ObjectPool.instance.SpawnFromPool(prefProj.type, playerPos, rotation, (o) => 
            { 
                Projectile p = o.GetComponent<Projectile>();
                if (p != null)
                {
                    p.Init(info.attackDamage, 10f); 
                    p.enemyLayer = enemyLayer;
                }

                BoomerangProjectile bp = o.GetComponent<BoomerangProjectile>();
                if (bp != null)
                {
                    bp.SetupBoomerang(travelDistance);
                }
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
