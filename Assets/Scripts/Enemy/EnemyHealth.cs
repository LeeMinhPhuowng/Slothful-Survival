using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine vfxRoutine;
    [SerializeField] float vfxExistTime;
    private Enemy enemy;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemy = GetComponent<Enemy>();
    }

    //Calculate damage taken
    public void TakeDamage(int damage)
    {
        TriggerTakeDamageVFX();
        enemy.DecreaseCurrentHealth(damage);
        if (enemy.GetCurrentHealth() <= 0)
        {
            Die();
            return;
        }
    }

    private void Die()
    {
        ObjectPool.instance.BackToPool(this.gameObject, enemy.info.type);

        //Sinh exp
        GameObject exp = ObjectPool.instance.SpawnFromPool(ObjectType.EXP, this.gameObject.transform.position);
        var expComponent = exp.GetComponent<EXP>();
        expComponent.Amount = enemy.info.expDrop;
        expComponent.AmountModifier();
    }


    //TakeDamage Effect
    private void TriggerTakeDamageVFX()
    {
        if (vfxRoutine != null)
        {
            StopCoroutine(vfxRoutine);
        }
        vfxRoutine = StartCoroutine(VFXCoroutine());
    }
    IEnumerator VFXCoroutine()
    {
        var block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block);
        block.SetFloat("_FlAmount", 1);
        spriteRenderer.SetPropertyBlock(block);

        yield return new WaitForSeconds(vfxExistTime);

        spriteRenderer.GetPropertyBlock(block);
        block.SetFloat("_FlAmount", 0);
        spriteRenderer.SetPropertyBlock(block);
    }
}
