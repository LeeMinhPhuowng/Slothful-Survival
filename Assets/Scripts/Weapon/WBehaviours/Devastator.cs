using UnityEngine;

public class Devastator : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject projectilePrefab;
    public override void Attack(Transform castPosition)
    {
        var enemies = Physics2D.OverlapCircleAll(castPosition.position, info.attackRange, enemyLayer);
        float distance = Mathf.Infinity;
        Enemy target = null;
        foreach (var enemy in enemies)
        {
            float current = Vector2.Distance(castPosition.position, enemy.gameObject.transform.position);
            if(current < distance)
            {
                distance = current;
                target = enemy.gameObject.GetComponentInParent<Enemy>();
            }
        }
        if (target != null)
        {
            Vector2 direction = (target.transform.position - castPosition.position).normalized;
            Quaternion rotation = Quaternion.FromToRotation(Vector2.right, direction);
            ObjectType type = projectilePrefab.GetComponent<Projectile>().type;

            var projectileObj = ObjectPool.instance.SpawnFromPool(type, castPosition.position, rotation, (o) => { o.GetComponent<Projectile>().Init(info.attackDamage, info.attackCooldown); });
            projectileObj.GetComponent<Projectile>().MoveForward();
        }      
    }
}
