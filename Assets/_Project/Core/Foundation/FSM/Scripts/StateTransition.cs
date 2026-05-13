using System;
using System.Text;

namespace Core.Foundation.FSM
{

    /// <summary>
    /// Represents a transition between two states, guarded by one or more conditions
    /// that must be satisfied for the transition to occur
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of context shared across states and transition guards.
    /// </typeparam>
    public class StateTransition<TContext>
    {
        /// <summary>
        /// The source state from which this transition originates.
        /// </summary>
        public IState<TContext> From { get; }

        /// <summary>
        /// The destination state to which this transition leads.
        /// </summary>
        public IState<TContext> To { get; }
        
        private readonly ITransitionGuard<TContext>[] _guards;
        
        /// <summary>
        /// Initializes a new state transition between two states with optional guards.
        /// </summary>
        public StateTransition(IState<TContext> from, IState<TContext> to, params ITransitionGuard<TContext>[] guards)
        {
            From = from;
            To = to;
            _guards = guards;
        }

        /// <summary>
        /// Determines whether this transition can be executed based on the current context
        /// </summary>
        public bool CanTransition(TContext context)
        {
            foreach (var guard in _guards)
            {
                if (!guard.Evaluate(context)) return false; 
            }

            return true;
        }


        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only debug string showing the transition and guard evaluation results.
        /// </summary>
        public string ToDebugString(TContext context)
        {
            string from = From?.GetType().Name ?? "Any";
            string to = To?.GetType().Name   ?? "<None>";

            if (_guards == null || _guards.Length == 0)
            {
                return $"{from} -> {to} (No Guards)";
            }

            StringBuilder sb = new();
            sb.AppendLine($"{from} -> {to}");

            foreach (var guard in _guards)
            {
                bool result = guard.Evaluate(context);
                sb.AppendLine($"  └─ [{(result ? "✓" : "✗")}] {guard.GetType().Name}");
            }

            return sb.ToString();
        }
        #endif
    }
}
