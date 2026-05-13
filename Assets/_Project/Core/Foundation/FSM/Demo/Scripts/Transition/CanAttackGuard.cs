namespace Core.Foundation.FSM.Demo
{

    public class CanAttackGuard : ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return context.Selector.IsTargetInAttackRange();
        }
    }
}
