using Pathfinding;
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

    //AI Pathfinding
    #region AI Pathfinding
    public AIPath AIPath { get; set; }
    public AIDestinationSetter DestinationSetter { get; set; }
    public Seeker Seeker { get; set; }
    public Transform TargetTransform { get; set; }
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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        AIPath = GetComponent<AIPath>();
        DestinationSetter = GetComponent<AIDestinationSetter>();
        Seeker = GetComponent<Seeker>();
        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        InitializeFromStat();
        DestinationSetter.target = PlayerInfo.instance.transform;
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
        AIPath.maxSpeed = MoveSpeed;
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

    //runs every 0.2 secs
    private void Tick()
    {
        if (AIPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (AIPath.desiredVelocity.x <= -0.01f)
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
        AIPath.canMove = false;
        animator.SetTrigger(attackTrigger);
        yield return new WaitForSeconds(0.8f);
        PlayerInfo.instance.TakeDamage(info.damage);
        Debug.Log(this.gameObject.name + "Attack!");
        yield return new WaitForSeconds(info.attackCooldown);
        isAttacking = false;
        AIPath.canMove = true;
    }

    #endregion
}
