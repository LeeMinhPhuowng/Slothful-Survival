using UnityEngine;

public class BaseStaff : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject projectilePrefab;
    public override void Attack(Transform castPosition)
    {
        var enemies = Physics2D.OverlapCircleAll(castPosition.position, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return;
        float minDistance = Mathf.Infinity;
        Collider2D target = null;
        foreach (var enemy in enemies)
        {
            if (Vector2.Distance(castPosition.position, enemy.transform.position) < minDistance)
            {
                minDistance = Vector2.Distance(castPosition.position, enemy.transform.position);
                target = enemy;
            }
        }
        if (target != null)
        {
            Vector2 direction = (target.transform.position - castPosition.position).normalized;
            Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, direction);
            ObjectType type = projectilePrefab.GetComponent<Projectile>().type;

            var projectileObj = ObjectPool.instance.SpawnFromPool(type, castPosition.position, quaternion, (o) => { o.GetComponent<Projectile>().Init(info.attackDamage, info.attackCooldown); });
            projectileObj.GetComponent<Projectile>().MoveForward();
        }
    }
}
