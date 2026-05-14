using System.Collections;
using UnityEngine;

public class Shield : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject shields;
    [SerializeField] float existTime;
    [SerializeField] float damageInterval = 0.5f;

    private GameObject shieldsObject;
    private bool _isActive = false;

    private void Start()
    {
        if (PlayerInfo.instance != null)
        {
            shieldsObject = Instantiate(shields, PlayerInfo.instance.transform.position, Quaternion.identity, PlayerInfo.instance.transform);
            shieldsObject.SetActive(false);
        }
    }

    public override bool Attack(Transform castPosition)
    {
        // If already active, don't start again and don't reset weapon cooldown yet
        if (_isActive) return false;

        if (PlayerInfo.instance == null) return false;
        
        var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;

        StartCoroutine(EnableShields());
        return true;
    }

    IEnumerator EnableShields()
    {
        _isActive = true;
        shieldsObject.SetActive(true);
        
        float timer = 0f;
        float lastDamageTime = -damageInterval;

        while (timer < existTime)
        {
            if (timer >= lastDamageTime + damageInterval)
            {
                DealDamage();
                lastDamageTime = timer;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        shieldsObject.SetActive(false);
        _isActive = false;
    }

    private void DealDamage()
    {
        var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
        foreach (var enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(info.attackDamage);
            }
        }
    }

    private void OnDestroy()
    {
        if (shieldsObject != null)
        {
            Destroy(shieldsObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (info == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, info.attackRange);
    }
}
