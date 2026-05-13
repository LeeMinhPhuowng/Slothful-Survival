namespace Core.Foundation.FSM.Demo
{

    public class IdlePressedGuard : ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return context.Input.IsIdlePressed;
        }
    }
}
