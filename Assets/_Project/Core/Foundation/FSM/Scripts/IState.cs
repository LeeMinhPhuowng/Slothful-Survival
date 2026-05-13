namespace Core.Foundation.FSM
{

    /// <summary>
    /// Represents a state in a finite state machine.
    /// Defines lifecycle callbacks that are invoked by the state machine.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context shared across all states of the state machine.
    /// </typeparam>
    public interface IState<TContext>
    {
        /// <summary>
        /// Called when the state becomes active.
        /// Used to initialize state-specific logic.
        /// </summary>
        void Enter(TContext context);

        /// <summary>
        /// Called when the state is exited.
        /// Used to clean up or reset state-specific data.
        /// </summary>
        void Exit(TContext context);

        /// <summary>
        /// Called every frame while the state is active.
        /// </summary>
        void Update(TContext context, float deltaTime);

        /// <summary>
        /// Called at a fixed time step while the state is active.
        /// Typically used for physics-related updates.
        /// </summary>
        void FixedUpdate(TContext context, float fixedDeltaTime);
    }

}
