namespace Core.Foundation.FSM.Demo
{

    public class WalkPressedGuard: ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return context.Input.IsWalkPressed;
        }
    }
}
