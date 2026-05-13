using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class WalkState : StateBase<CubeContext>
    {
        public override void Enter(CubeContext context)
        {
        }

        public override void Update(CubeContext context, float deltaTime)
        {
            context.Input.Tick(deltaTime);
            context.Movement.MoveDirection(Vector3.down, deltaTime);
            context.Selector.TrySelectTargetInRange();
        }

        public override void Exit(CubeContext context)
        {
            
        }
    }
}
