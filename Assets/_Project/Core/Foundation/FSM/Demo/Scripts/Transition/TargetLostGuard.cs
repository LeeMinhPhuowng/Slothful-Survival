namespace Core.Foundation.FSM.Demo
{

    public class TargetLostGuard : ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return !context.Selector.IsTargetInDetectRange();
        }
    }
}
