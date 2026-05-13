
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR.Haptics;
using static UnityEngine.Rendering.DebugUI;

public class Enemy : MonoBehaviour, IDamageable
{
    //Info
    public EnemyInfoSO info;

    //Take Damage VFX
    #region Take Damage VFX
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine vfxRoutine;
    [SerializeField] float vfxExistTime;
    #endregion



    //Properties
    public float MaxHealth { get; set; } //Not use
    public float CurrentHealth { get; set; }
    public int MoveSpeed { get; set; }
    public float AttackRange { get; set; }

    private Animator animator;
    private Rigidbody2D rb;

    private int attackTrigger = Animator.StringToHash("Attack");

    private bool isAttacking = false;
    private bool canMove = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        canMove = true;
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    private void OnEnable()
    {
        InitializeFromStat();
        Ticker.OnTickAction += Tick;
        GameManager.EnemyCount++;
    }

    private void OnDisable()
    {
        Ticker.OnTickAction -= Tick;
        GameManager.EnemyCount--;
    }

    private void InitializeFromStat()
    {
        CurrentHealth = info.maxHealth;
        MoveSpeed = info.moveSpeed;
    }

    public void TakeDamage(int damage)
    {
        TriggerTakeDamageVFX();
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }
    }

    public void Die()
    {
        ObjectPool.instance.BackToPool(this.gameObject, info.type);

        //Sinh exp
        GameObject exp = ObjectPool.instance.SpawnFromPool(ObjectType.EXP, this.gameObject.transform.position);
        var expComponent = exp.GetComponent<EXP>();
        expComponent.Amount = info.expDrop;
        expComponent.AmountModifier();
    }
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!canMove || isAttacking || PlayerInfo.instance == null) return;

        Vector2 direction = (PlayerInfo.instance.transform.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * (MoveSpeed * Time.fixedDeltaTime));
    }

    //runs every 0.2 secs
    private void Tick()
    {
        if (PlayerInfo.instance == null) return;

        float dirX = PlayerInfo.instance.transform.position.x - transform.position.x;
        if (dirX >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (dirX <= -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (Vector2.Distance(this.transform.position, PlayerInfo.instance.transform.position) <= info.attackRange && isAttacking == false)
        {
            Debug.Log("In range");
            StartCoroutine(EnemyAttack());
        }
    }
    
    #region TakeDmgVFX Coroutine
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
    #endregion

    #region EnemyAttack
    IEnumerator EnemyAttack()
    {
        isAttacking = true;
        canMove = false;
        animator.SetTrigger(attackTrigger);
        yield return new WaitForSeconds(0.8f);
        PlayerInfo.instance.TakeDamage(info.damage);
        Debug.Log(this.gameObject.name + "Attack!");
        yield return new WaitForSeconds(info.attackCooldown);
        isAttacking = false;
        canMove = true;
    }

    #endregion
}
