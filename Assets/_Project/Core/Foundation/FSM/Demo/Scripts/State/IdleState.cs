namespace Core.Foundation.FSM.Demo
{

    public class IdleState : StateBase<CubeContext>
    {
        public override void Enter(CubeContext context)
        {
            context.Combat.StartCooldown(); // Should belong in the AttackState, but placed here for demo purposes
        }

        public override void Update(CubeContext context, float deltaTime)
        {
            context.Input.Tick(deltaTime);
            context.Combat.Tick(deltaTime);
            context.Selector.TrySelectTargetInRange();
        }

        public override void Exit(CubeContext context)
        {
            
        }
    }
}
