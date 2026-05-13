namespace Core.Foundation.FSM.Demo
{

    public class IsDeadGuard : ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return context.Health.IsDead;
        }
    }
}
