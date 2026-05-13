using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class HistoryTest
    {
        [Test]
        public void ToDebugString_WhenStateTransitionOccurs_ReturnsCorrectOrder()
        {
            List<Vector3> targets = new(){ Vector3.one, new(1, -3, 0), new(10, 10, 0) };
            var context = new CubeContext(new FakePosition(Vector3.zero), new FakeDetector(targets), new FakeCubeConfig());

            var walkState = new WalkState();
            var attackState = new AttackState();

            var stateMachine = StateMachineFactory.Create(context, walkState, 5);
            stateMachine.AddTransition(walkState, attackState, new AttackPressedGuard());

            context.Input.HandleKey(KeyCode.A);

            stateMachine.Update(0.1f);

            var expectedResult = $"{walkState.GetType().Name} -> {attackState.GetType().Name}";

            Assert.AreEqual(stateMachine.History.ToDebugString(), expectedResult);
        }

        [Test]
        public void ToDebugString_WhenExceedCapacity_ReturnsLatestStatesOnly()
        {
            List<Vector3> targets = new(){ Vector3.one, new(1, -3, 0), new(10, 10, 0) };
            var context = new CubeContext(new FakePosition(Vector3.zero), new FakeDetector(targets), new FakeCubeConfig());
        
            var walkState = new WalkState();
            var attackState = new AttackState();
            var chaseState = new ChaseState();
            var idleState = new IdleState();
            var dieState = new DieState();

            var stateMachine = StateMachineFactory.Create(context, walkState, 3);
            stateMachine.AddTransition(null, dieState, new IsDeadGuard());

            stateMachine.AddTransition(walkState, chaseState, new HasTargetInRangeGuard());

            stateMachine.AddTransition(idleState, attackState, new CooldownFinishedGuard(), new CanAttackGuard());
            stateMachine.AddTransition(idleState, chaseState, new CooldownFinishedGuard(), new HasTargetInRangeGuard());
            stateMachine.AddTransition(chaseState, attackState, new CanAttackGuard());

            stateMachine.AddTransition(attackState, idleState, new AttackFinishedGuard());
            stateMachine.AddTransition(chaseState, idleState, new TargetLostGuard());
            stateMachine.AddTransition(idleState, walkState, new CooldownFinishedGuard(), new TargetLostGuard());

            var historyResult = new StateHistory<CubeContext>(3);
            historyResult.Record(walkState); // FSM was initialized with WalkState

            Assert.AreEqual(stateMachine.History.ToDebugString(), historyResult.ToDebugString());

            stateMachine.Update(0.1f); // TryTransition to ChaseState
            historyResult.Record(chaseState);

            Assert.AreEqual(stateMachine.History.ToDebugString(), historyResult.ToDebugString());

            stateMachine.Update(2f); // Allow Cube Move to Target
            stateMachine.Update(0.1f); // TryTransition to AttackState
            historyResult.Record(attackState);

            Assert.AreEqual(stateMachine.History.ToDebugString(), historyResult.ToDebugString());
            
            stateMachine.Update(5f); // Wait for attack duraion, attackDuration has value auto equal 5f in FakeCubeConfig
            stateMachine.Update(0.1f); // TryTransition to Idle State

            historyResult.Record(idleState);
            
            Assert.AreEqual(stateMachine.History.ToDebugString(), historyResult.ToDebugString());

            stateMachine.Update(0.1f); // TryTransition to Die State
            historyResult.Record(dieState);

            Assert.AreEqual(stateMachine.History.ToDebugString(), historyResult.ToDebugString());
        }
    }
}
