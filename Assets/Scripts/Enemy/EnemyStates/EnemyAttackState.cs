using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private float attackTimer;
    private float attackDuration;
    private bool hasDealtDamage;

    public EnemyAttackState(Enemy enemy, EnemyStateMachine esm) : base(enemy, esm) { }

    public override void EnterState()
    {
        Debug.Log(enemy.gameObject.name + " entered AttackState");
        attackTimer = 0f;
        hasDealtDamage = false;
        enemy.SetBoolAnimation("Attack", true);
        attackDuration = 0.5f;
        enemy.ResetVelocity();
    }

    public override void ExitState()
    {
        Debug.Log(enemy.gameObject.name + " exited AttackState");
        enemy.SetBoolAnimation("Attack", false);
    }

    public override void FrameUpdate()
    {
        attackTimer += Time.deltaTime;

        if (Vector2.Distance(PlayerInfo.instance.gameObject.transform.position, enemy.transform.position) > enemy.info.attackRange)
        {
            enemy.EnemyStateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        if (!hasDealtDamage && attackTimer >= attackDuration)
        {
            hasDealtDamage = true;
            PlayerInfo.instance.TakeDamage(enemy.info.damage);
            Debug.Log("Player received damage from" + enemy.gameObject.name);
            enemy.EnemyStateMachine.ChangeState(enemy.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
