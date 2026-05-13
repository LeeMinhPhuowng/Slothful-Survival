using UnityEngine;

namespace Core.Foundation.FSM
{

    /// <summary>
    /// Provides factory methods for creating and initializing state machine instances.
    /// </summary>
    public class StateMachineFactory
    {
        /// <summary>
        /// Creates and initializes a standalone state machine instance.
        /// </summary>
        public static StateMachine<TContext> Create<TContext>(TContext context, IState<TContext> initialState, int historyCapacity)
        {
            StateMachine<TContext> stateMachine = new(context, historyCapacity);
            stateMachine.Initialize(initialState);
            return stateMachine;
        }
    }
}
