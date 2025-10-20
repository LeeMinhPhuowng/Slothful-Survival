using UnityEditor;
using UnityEngine;

public class EnemyAttackState : EnemyState
{

    public EnemyAttackState(Enemy enemy, EnemyStateMachine esm) : base(enemy, esm)
    {
    }

    public override void TriggerAnimationEvent()
    {
        Debug.Log(enemy.gameObject.name + "triggered AnimEvent");
        PlayerInfo.instance.TakeDamage(enemy.info.damage);
        enemyStateMachine.ChangeState(enemy.IdleState);
    }

    public override void EnterState()
    {
        Debug.Log(enemy.gameObject.name + "entered AttackState");
        enemy.SetTriggerAnimation("Attack");
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
