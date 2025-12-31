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

    //State Variables
    public EnemyStateMachine EnemyStateMachine { get; set; }

    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemyIdleState IdleState { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        AIPath = GetComponent<AIPath>();
        DestinationSetter = GetComponent<AIDestinationSetter>();
        Seeker = GetComponent<Seeker>();

        EnemyStateMachine = new EnemyStateMachine();

        ChaseState = new EnemyChaseState(this, EnemyStateMachine);
        AttackState = new EnemyAttackState(this, EnemyStateMachine);
        IdleState = new EnemyIdleState(this, EnemyStateMachine);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        InitializeFromStat();
    }

    private void InitializeFromStat()
    {
        CurrentHealth = info.maxHealth;
        MoveSpeed = info.moveSpeed;
        EnemyStateMachine.Initialize(ChaseState);
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

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    #region Animation Actions
    public void SetBoolAnimation(string animation, bool value)
    {
        animator.SetBool(animation, value);
    }

    public void SetTriggerAnimation(string animation)
    {
        animator.SetTrigger(animation);
    }

    public void TriggerAnimEvent()
    {

    }
    #endregion

    #region Updates
    private void Update()
    {
        EnemyStateMachine.CurrentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        EnemyStateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

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
}
