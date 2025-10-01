/*
using UnityEngine;
[CreateAssetMenu(fileName = "Base Bow", menuName = "Weapon/Weapons/Base Bow")]
public class BaseBowSO : WeaponInfoSO
{
    [SerializeField] GameObject projectilePrefab;
    public LayerMask enemyLayer;
    public override void Attack(Transform castPosition)
    {
        var enemies = Physics2D.OverlapCircleAll(castPosition.position, attackRange, enemyLayer);
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
            Instantiate(projectilePrefab, castPosition.position, quaternion);
        }

    }
}
*/