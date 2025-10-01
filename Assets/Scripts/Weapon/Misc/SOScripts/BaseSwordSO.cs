/*
using UnityEngine;
[CreateAssetMenu(fileName = "Base Sword", menuName = "Weapon/Weapons/Base Sword")]
public class BaseSwordSO : WeaponInfoSO
{
    public LayerMask enemyLayer;
    [SerializeField] GameObject SlashVFX;
    [SerializeField] float VFXExistTime = 0.35f;
    public override void Attack(Transform castPosition)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(castPosition.position, attackRange, enemyLayer);
        if (enemies.Length == 0) return;
        float minDistance = Mathf.Infinity;
        Collider2D target = null;
        foreach(var enemy in enemies)
        {
            if(Vector2.Distance(castPosition.position, enemy.transform.position) < minDistance)
            {
                minDistance = Vector2.Distance(castPosition.position, enemy.transform.position);
                target = enemy;
            }
        }
        if(target != null)
        {
            IDamageable dmg = target.GetComponentInParent<IDamageable>();
            dmg.TakeDamage(attackDamage);

            //Debug.Log("Slash!");
            Enemy enemy = target.GetComponentInParent<Enemy>();
            GameObject vfx = Instantiate(SlashVFX, enemy.VFXPlayer.transform.position, Quaternion.Euler(0f, 180f, 0f), target.gameObject.transform);
            Destroy(vfx, VFXExistTime);
        }       
    }
}
*/