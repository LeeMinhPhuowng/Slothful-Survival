using System.Collections;
using UnityEngine;

public class PlayerInfo : MonoBehaviour, IDamageable
{
    private float maxHealth;
    private float currentHealth;
    private float moveSpeed;
    private float pickupRange;
    private int currentLevel;
    [SerializeField] float basePickupRange;

    private UpdateValue healthBarValue;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine vfxRoutine;

    [SerializeField] float vfxExistTime;

    public static PlayerInfo instance;

    private void Awake()
    {
        instance = this;    
        spriteRenderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
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
        MoveSpeed = characterInfo.moveSpeed;
        PickupRange = basePickupRange;
        CurrentLevel = 0;
        healthBarValue = HealthBarCanvas.Instance.gameObject.GetComponentInChildren<UpdateValue>();
    }

    //Calculate received damage
    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        this.gameObject.SetActive(false);
    }

    //Check collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            TakeDamage(enemy.info.damage);
            TriggerTakeDamageVFX();
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
