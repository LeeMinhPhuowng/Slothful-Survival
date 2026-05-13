namespace Core.Foundation.FSM
{

    /// <summary>
    /// Defines a base class for states with optional lifecycle callbacks.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context used by the state machine and shared across all states.
    /// </typeparam>
    public abstract class StateBase<TContext> : IState<TContext>
    {
        /// <summary>
        /// Called when the state becomes active.
        /// </summary>
        public virtual void Enter(TContext context) {}

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public virtual void Exit(TContext context) {}

        /// <summary>
        /// Called every frame while the state is active.
        /// </summary>
        public virtual void Update(TContext context, float deltaTime) {}

        /// <summary>
        /// Called at a fixed time step while the state is active.
        /// </summary>
        public virtual void FixedUpdate(TContext context, float fixedDeltaTime) {}
    }
}
