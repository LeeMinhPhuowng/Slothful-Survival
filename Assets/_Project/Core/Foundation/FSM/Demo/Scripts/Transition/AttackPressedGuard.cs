namespace Core.Foundation.FSM.Demo
{

    public class AttackPressedGuard : ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return context.Input.IsAttackPressed;
        }
    }
}
