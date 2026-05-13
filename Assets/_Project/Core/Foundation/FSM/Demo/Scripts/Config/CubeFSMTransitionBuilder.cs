namespace Core.Foundation.FSM.Demo
{

    public static class CubeFSMTransitionBuilder
    {
        public static void Build(StateMachine<CubeContext> stateMachine, WalkState walkState, 
            ChaseState chaseState, AttackState attackState, IdleState idleState, DieState dieState)
        {
            stateMachine.AddTransition(null, walkState, new WalkPressedGuard());
            stateMachine.AddTransition(null, attackState, new AttackPressedGuard());
            stateMachine.AddTransition(null, idleState, new IdlePressedGuard());

            stateMachine.AddTransition(null, dieState, new IsDeadGuard());

            stateMachine.AddTransition(walkState, chaseState, new HasTargetInRangeGuard());

            stateMachine.AddTransition(idleState, attackState, new CooldownFinishedGuard(), new CanAttackGuard());
            stateMachine.AddTransition(idleState, chaseState, new CooldownFinishedGuard(), new HasTargetInRangeGuard());
            stateMachine.AddTransition(chaseState, attackState, new CanAttackGuard());

            stateMachine.AddTransition(attackState, idleState, new AttackFinishedGuard());
            stateMachine.AddTransition(chaseState, idleState, new TargetLostGuard());
            stateMachine.AddTransition(idleState, walkState, new CooldownFinishedGuard(), new TargetLostGuard());
        }
    }
}
