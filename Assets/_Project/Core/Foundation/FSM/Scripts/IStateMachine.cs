namespace Core.Foundation.FSM
{

    /// <summary>
    /// Represents a finite state machine.
    /// Responsible for managing state transitions and driving state updates.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context shared across all states and transition guards.
    /// </typeparam>
    public interface IStateMachine<TContext>
    {
        /// <summary>
        /// Gets the currently active state of the state machine.
        /// </summary>
        IState<TContext> CurrentState { get; }

        /// <summary>
        /// Transitions the state machine to a new state.
        /// </summary>
        void ChangeState(IState<TContext> newState);

        /// <summary>
        /// Updates the active state once per frame.
        /// Typically called from a game loop or update cycle.
        /// </summary>
        void Update(float deltaTime);

        /// <summary>
        /// Updates the active state at a fixed time step.
        /// Typically used for deterministic or physics-related logic.
        /// </summary>
        void FixedUpdate(float fixedDeltaTime);
    }
}
