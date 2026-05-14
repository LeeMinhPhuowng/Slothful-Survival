using System.Collections;
using Game.UI.Data;
using Game.UI.Model;
using Game.UI.Service;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour, IDamageable
{
    //Properties
    private float maxHealth;
    private float currentHealth;
    private float moveSpeed;
    private float pickupRange;
    private float bonusAttack;
    private float armor;
    private int currentLevel;
    private bool isDead;

    [SerializeField] float basePickupRange;

    private HealthBarValue healthBarValue;

    //Flash Effect
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine vfxRoutine;

    [SerializeField] float vfxExistTime;
    [SerializeField] float invincibilityDuration = 0.5f;
    private float lastDamageTime;

    public static PlayerInfo instance;

    private void Awake()
    {
        instance = this;    
        spriteRenderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        //healthBarValue.SetHealth(GetCurrentHealthPercentage());
    }
    
    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value; 
        }
    }

    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
        }
    }

    public float PickupRange
    {
        get
        {
            return pickupRange;
        }
        set
        {
            pickupRange = value;
        }
    }

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
        set
        {
            currentLevel = value;
        }
    }
    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
        }
    }
    public float BonusAttack
    {
        get
        {
            return bonusAttack;
        }
        set
        {
            bonusAttack = value;
        }
    }
    public float Armor
    {
        get
        {
            return armor;
        }
        set
        {
            armor = value;
        }
    }

    private float GetCurrentHealthPercentage()
    {
        return (float)CurrentHealth / MaxHealth;
    }

    //Init
    public void InitializeFromCharacterInfoSO(CharacterInfoSO characterInfo)
    {
        MaxHealth = characterInfo.maxHealth;
        CurrentHealth = MaxHealth;
        MoveSpeed = characterInfo.moveSpeed;
        isDead = false;
        PickupRange = basePickupRange;
        CurrentLevel = 0;
        //healthBarValue = HealthBarCanvas.Instance.gameObject.GetComponentInChildren<HealthBarValue>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }
        CurrentHealth -= (amount - armor / 10); //Hard code temporarily
        TriggerTakeDamageVFX();
        //healthBarValue.SetHealth(GetCurrentHealthPercentage());
        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        GameplayRunSignals.ReportPlayerDied();
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        // If we recently took damage, don't take it again yet
        if (Time.time < lastDamageTime + invincibilityDuration) return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy == null) enemy = collision.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                TakeDamage(enemy.info.damage);
                lastDamageTime = Time.time; // Start I-Frame
            }
        }
    }

    public void Revive(float healthPercent)
    {
        isDead = false;
        CurrentHealth = MaxHealth * Mathf.Clamp01(healthPercent);
        if (healthBarValue != null)
        {
            healthBarValue.SetHealth(GetCurrentHealthPercentage());
        }
    }

    //TakeDamage Effect
    private void TriggerTakeDamageVFX()
    {
        if(vfxRoutine != null)
        {
            StopCoroutine(vfxRoutine);
        }
        vfxRoutine = StartCoroutine(VFXCoroutine());
    }
    IEnumerator VFXCoroutine()
    {
        float duration = vfxExistTime > 0 ? vfxExistTime : 0.1f;
        mpb.SetFloat("_FlAmount", 1);
        spriteRenderer.SetPropertyBlock(mpb);
        yield return new WaitForSeconds(duration);
        mpb.SetFloat("_FlAmount", 0);
        spriteRenderer.SetPropertyBlock(mpb);
    }

}
