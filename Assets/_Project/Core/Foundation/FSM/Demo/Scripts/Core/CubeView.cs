using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class CubeView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void Bind(StateMachine<CubeContext> stateMachine)
        {
            stateMachine.OnStateEntered += Colored;
            
            if (stateMachine.CurrentState != null)
            {
                Colored(stateMachine.CurrentState);
            }

        }

        private void Colored(IState<CubeContext> state)
        {
            if (state is WalkState)
            {
                _renderer.color = Color.yellow;
            }
            else if (state is ChaseState)
            {
                _renderer.color = Color.crimson;
            }
            else if (state is AttackState)
            {
                _renderer.color = Color.tomato;
            }
            else if (state is IdleState)
            {
                _renderer.color = Color.blueViolet;
            }
            else if (state is DieState)
            {
                _renderer.color = Color.clear;
            }
            else _renderer.color = Color.white;
        }
    }
}
