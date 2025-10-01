using UnityEngine;

public class BaseSword : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject slashVFX;
    [SerializeField] float VFXExistTime = 0.35f;

    public override void Attack(Transform castPosition)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(castPosition.position, info.attackRange, enemyLayer);
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
            Enemy enemy = target.GetComponentInParent<Enemy>();
            GameObject vfx = Instantiate(slashVFX, enemy.VFXPlayer.transform.position, Quaternion.Euler(0f, 180f, 0f));
            enemy.ReceiveDamage(info.attackDamage);
            Destroy(vfx, VFXExistTime);
        }       
    }
}



