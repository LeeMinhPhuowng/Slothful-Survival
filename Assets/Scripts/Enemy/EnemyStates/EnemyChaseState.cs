using Pathfinding;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(Enemy enemy, EnemyStateMachine esm) : base(enemy, esm) { }

    public override void TriggerAnimationEvent()
    {
        base.TriggerAnimationEvent();
    }

    public override void EnterState()
    {
        Debug.Log(enemy.gameObject.name + "entered ChaseState");
        enemy.DestinationSetter.target = GameObject.FindGameObjectWithTag("Player").transform;
        enemy.AIPath.maxSpeed = enemy.MoveSpeed;
        enemy.AIPath.canMove = true;
        enemy.DestinationSetter.enabled = true;
        enemy.AIPath.enabled = true;
        enemy.Seeker.enabled = true;
    }

    public override void ExitState()
    {
        Debug.Log(enemy.gameObject.name + "exited ChaseState");
        enemy.AIPath.maxSpeed = 0;
        enemy.AIPath.canMove = false;
        enemy.DestinationSetter.enabled = false;
        enemy.AIPath.enabled = false;
        enemy.Seeker.enabled = false;
        enemy.ResetVelocity();
    }

    public override void FrameUpdate()
    {
        if (enemy.AIPath.desiredVelocity.x >= 0.01f)
        {
            enemy.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else enemy.transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    public override void PhysicsUpdate()
    {

    }
}
