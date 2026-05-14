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
    private int currentLevel;
    [SerializeField] float basePickupRange;

    private HealthBarValue healthBarValue;

    //Flash Effect
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine vfxRoutine;
    private bool isDead;

    [SerializeField] float vfxExistTime;

    public static PlayerInfo instance;

    private void Awake()
    {
        instance = this;    
        spriteRenderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    [Inject]
    private void Construct(InventoryService inventoryService)
    {
        foreach ((EquipmentSlot slot, EquipmentItemModel item) in inventoryService.EquippedItems)
        {
            if (item == null)
            {
                continue;
            }

            // Cong chi so tu Item
        }
    }

    private void Update()
    {
        healthBarValue.SetHealth(GetCurrentHealthPercentage());
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

    private float GetCurrentHealthPercentage()
    {
        return (float)CurrentHealth / MaxHealth;
    }

    //Init
    public void InitializeFromCharacterInfoSO(CharacterInfoSO characterInfo)
    {
        MaxHealth = characterInfo.maxHealth;
        CurrentHealth = MaxHealth;
        isDead = false;
        MoveSpeed = characterInfo.moveSpeed;
        PickupRange = basePickupRange;
        CurrentLevel = 0;
        healthBarValue = HealthBarCanvas.Instance.gameObject.GetComponentInChildren<HealthBarValue>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        CurrentHealth -= amount;
        TriggerTakeDamageVFX();
        healthBarValue.SetHealth(GetCurrentHealthPercentage());
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
        mpb.SetFloat("_FlAmount", 1);
        spriteRenderer.SetPropertyBlock(mpb);
        yield return new WaitForSeconds(vfxExistTime);
        mpb.SetFloat("_FlAmount", 0);
        spriteRenderer.SetPropertyBlock(mpb);
    }

}
