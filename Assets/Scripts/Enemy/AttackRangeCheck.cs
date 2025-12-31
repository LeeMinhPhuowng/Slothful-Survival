using UnityEngine;

public class AttackRangeCheck : MonoBehaviour
{
    Enemy enemy;
    EnemyStateMachine enemyStateMachine;

    private void Awake()
    {
        enemy = this.gameObject.GetComponentInParent<Enemy>();
        enemyStateMachine = enemy.EnemyStateMachine;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyStateMachine.ChangeState(enemy.IdleState);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !(enemyStateMachine.CurrentState is EnemyAttackState))
            enemyStateMachine.ChangeState(enemy.ChaseState);
    }
}
