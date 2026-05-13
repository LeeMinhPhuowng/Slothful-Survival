namespace Core.Foundation.FSM.Demo
{

    public class HasTargetInRangeGuard : ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return context.Selector.IsTargetInDetectRange();
        }
    }
}
