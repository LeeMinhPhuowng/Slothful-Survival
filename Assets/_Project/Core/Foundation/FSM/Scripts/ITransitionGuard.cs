namespace Core.Foundation.FSM
{

    /// <summary>
    /// Defines a condition that controls whether a state transition is allowed.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context used by the state machine to evaluate transition conditions.
    /// </typeparam>
    public interface ITransitionGuard<TContext>
    {
        /// <summary>
        /// Evaluates whether the transition condition is satisfied.
        /// </summary>
        bool Evaluate(TContext context); 
    }
}
