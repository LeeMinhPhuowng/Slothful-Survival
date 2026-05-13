namespace Core.Foundation.FSM.Demo
{

    public class AttackFinishedGuard : ITransitionGuard<CubeContext>
    {
        public bool Evaluate(CubeContext context)
        {
            return context.Combat.IsAttackFinished;
        }
    }
}
