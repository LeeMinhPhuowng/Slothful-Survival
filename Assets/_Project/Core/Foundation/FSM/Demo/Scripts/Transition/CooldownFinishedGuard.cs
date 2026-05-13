namespace Core.Foundation.FSM.Demo
{

    public class CooldownFinishedGuard : ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return context.Combat.IsCooldownFinished;   
        }
    }
}
