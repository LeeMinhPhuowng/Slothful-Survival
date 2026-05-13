using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class GuardTest
    {
        [Test]
        public void Evaluate_IdleKeyPressed_ReturnsTrue()
        {
            List<Vector3> targets = new(){ Vector3.one, new(1, -3, 0), new(10, 10, 0) };
            var context = new CubeContext(new FakePosition(Vector3.zero), new FakeDetector(targets), new FakeCubeConfig());

            context.Input.HandleKey(KeyCode.I);
            
            var idleGuard = new IdlePressedGuard();
            var result = idleGuard.Evaluate(context);

            Assert.IsTrue(result);
        }
        
        [Test]
        public void Evaluate_WhenTargetInRange_ReturnsTrue()
        {
            List<Vector3> targets = new(){ Vector3.one, new(1, -3, 0), new(10, 10, 0) };
            var context = new CubeContext(new FakePosition(Vector3.zero), new FakeDetector(targets), new FakeCubeConfig());

            context.Selector.TrySelectTargetInRange();

            var guard = new HasTargetInRangeGuard();
            var result = guard.Evaluate(context);

            Assert.IsTrue(result);
        }

        // Normal guard test that depends on the current FSM state
        [Test]
        public void Evaluate_WhenIsDead_ReturnsTrue()
        {
            List<Vector3> targets = new(){ Vector3.one, new(1, -3, 0), new(10, 10, 0) };
            var context = new CubeContext(new FakePosition(Vector3.zero), new FakeDetector(targets), new FakeCubeConfig());
            
            AttackState attackState = new();
            IdleState idleState = new();

            var stateMachine = StateMachineFactory.Create(context, attackState, 5);

            // Demo behavior: AttackState.Exit() decreases HP
            stateMachine.ChangeState(idleState);
            
            var guard = new IsDeadGuard();
            var result = guard.Evaluate(context);

            Assert.IsTrue(result);
        }
    }
}
