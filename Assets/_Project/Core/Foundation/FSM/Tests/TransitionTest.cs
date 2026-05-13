using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class TransitionTest
    {
        [Test]
        public void TryTransition_WhenAttackKeyPressedInWalkState_ChangesToAttackState()
        {
            List<Vector3> targets = new(){ Vector3.one, new(1, -3, 0), new(10, 10, 0) };
            var context = new CubeContext(new FakePosition(Vector3.zero), new FakeDetector(targets), new FakeCubeConfig());

            var walkState = new WalkState();
            var attackState = new AttackState();

            var stateMachine = StateMachineFactory.Create(context, walkState, 5);
            stateMachine.AddTransition(walkState, attackState, new AttackPressedGuard());

            context.Input.HandleKey(KeyCode.A);

            stateMachine.Update(0.1f);
            Assert.IsTrue(stateMachine.CurrentState is AttackState);
        }

        [Test]
        public void TryTransition_WhenTargetInRangeAndCanAttack_ChangesToAttackState()
        {
            List<Vector3> targets = new(){ Vector3.one, new(1, -3, 0), new(10, 10, 0) };
            var context = new CubeContext(new FakePosition(Vector3.zero), new FakeDetector(targets), new FakeCubeConfig());

            var walkState = new WalkState();
            var chaseState = new ChaseState();
            var attackState = new AttackState();

            var stateMachine = StateMachineFactory.Create(context, walkState, 5);
            stateMachine.AddTransition(walkState, chaseState, new HasTargetInRangeGuard());
            stateMachine.AddTransition(chaseState, attackState, new CanAttackGuard());
            
            stateMachine.Update(0.1f); // StateMachine Try Transition to Chase State
            Assert.IsTrue(stateMachine.CurrentState is ChaseState);

            stateMachine.Update(2f); // Allow Cube Move to Target
            stateMachine.Update(0.1f); // StateMachine Try Transition again
            Assert.IsTrue(stateMachine.CurrentState is AttackState);
        }

        [Test]
        public void TryTransition_WhenIsDeadFromAnyState_ChangesToDieState()
        {
            List<Vector3> targets = new(){ Vector3.one, new(3, -3, 0), new(10, 10, 0) };
            var context = new CubeContext(new FakePosition(Vector3.zero), new FakeDetector(targets), new FakeCubeConfig());

            var attackState = new AttackState();
            var walkState = new WalkState();
            var dieState = new DieState();

            var stateMachine = StateMachineFactory.Create(context, attackState, 5);
            stateMachine.AddTransition(attackState, walkState, new TargetLostGuard());
            stateMachine.AddTransition(null, dieState, new IsDeadGuard());

            stateMachine.Update(0.1f);
            Assert.IsTrue(stateMachine.CurrentState is WalkState);

            stateMachine.Update(0.1f);
            Assert.IsTrue(stateMachine.CurrentState is DieState);
        }
    }
}
