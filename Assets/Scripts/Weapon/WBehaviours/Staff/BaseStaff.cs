using UnityEngine;

public class BaseStaff : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject projectilePrefab;
    public override bool Attack(Transform castPosition)
    {
        if (PlayerInfo.instance == null) return false;
        Vector3 playerPos = PlayerInfo.instance.transform.position;

        var enemies = Physics2D.OverlapCircleAll(playerPos, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;
        
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
            Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, direction);
            ObjectType type = projectilePrefab.GetComponent<Projectile>().type;

            var projectileObj = ObjectPool.instance.SpawnFromPool(type, playerPos, quaternion, (o) => 
            { 
                Projectile p = o.GetComponent<Projectile>();
                p.Init(info.attackDamage, info.attackCooldown); 
                p.enemyLayer = enemyLayer;
            });
            projectileObj.GetComponent<Projectile>().MoveForward();
            return true;
        }
        return false;
    }
}
