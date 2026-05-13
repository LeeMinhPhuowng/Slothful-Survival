namespace Core.Foundation.FSM.Demo
{

    public class DieState : StateBase<CubeContext>
    {
        public override void Enter(CubeContext context)
        {
        }

        public override void Update(CubeContext context, float deltaTime)
        {
            context.Input.Tick(deltaTime);
        }

        public override void Exit(CubeContext context)
        {
            
        }
    }
}
