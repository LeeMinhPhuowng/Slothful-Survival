namespace Core.Foundation.FSM.Demo
{

    public class AttackState : StateBase<CubeContext>
    {
        public override void Enter(CubeContext context)
        {
            context.Combat.StartAttackDuration();
        }

        public override void Update(CubeContext context, float deltaTime)
        {
            context.Input.Tick(deltaTime);
            context.Combat.Tick(deltaTime);
        }

        public override void Exit(CubeContext context)
        {
            context.Health.Damage(1); 
            context.Selector.ResetTarget(); // Reset Target for demo purposes
        }
    }
}
