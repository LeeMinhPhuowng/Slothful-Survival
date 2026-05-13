namespace Core.Foundation.FSM.Demo
{

    public class ChaseState : StateBase<CubeContext>
    {
        public override void Enter(CubeContext context)
        {
        }

        public override void Update(CubeContext context, float deltaTime)
        {
            context.Input.Tick(deltaTime);
            context.Movement.MoveToward(context.Selector.Target.Position, deltaTime);
        }

        public override void Exit(CubeContext context)
        {
            
        }
    }
}
