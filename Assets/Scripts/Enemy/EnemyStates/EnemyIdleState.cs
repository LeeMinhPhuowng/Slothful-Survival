/*
public class EnemyIdleState : EnemyState
{
    float timeElapsed;

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void TriggerAnimationEvent()
    {
        base.TriggerAnimationEvent();
    }

    public override void EnterState()
    {
        Debug.Log(enemy.gameObject.name + "entered IdleState");
        timeElapsed = 0f;
        enemy.SetBoolAnimation("Idle", true);
    }

    public override void ExitState()
    {
        Debug.Log(enemy.gameObject.name + "exited IdleState");  
        enemy.SetBoolAnimation("Idle", false);
    }

    public override void FrameUpdate()
    {
        if (Vector2.Distance(PlayerInfo.instance.gameObject.transform.position, enemy.transform.position) > enemy.info.attackRange)
        {
            enemy.EnemyStateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        timeElapsed += Time.deltaTime;
        if (timeElapsed >= enemy.info.attackCooldown)
        {
            enemyStateMachine.ChangeState(enemy.AttackState);
            timeElapsed = 0;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }


}
*/