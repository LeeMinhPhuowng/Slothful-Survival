using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Foundation.FSM
{

    /// <summary>
    /// Stores a limited history of visited states for debugging and inspection purposes.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context type associated with the recorded states.
    /// </typeparam>
    public class StateHistory<TContext>
    {
        private readonly int _capacity;
        private readonly Queue<IState<TContext>> _history;
        private readonly List<Type> _stateTypes;

        /// <summary>
        /// Read-only history of states visited by the state machine.
        /// </summary>
        public IReadOnlyCollection<IState<TContext>> States => _history;

        /// <summary>
        /// Non-generic view of the state history for debugging purposes.
        /// </summary>
        public IReadOnlyCollection<Type> StateTypes => _stateTypes;
        /// <summary>
        /// Creates a new state history with a fixed capacity.
        /// </summary>
        public StateHistory(int capacity)
        {
            _capacity = capacity;
            _history = new(capacity);
            _stateTypes = new();
        }

        /// <summary>
        /// Records a state into the history.
        /// If the capacity is exceeded, the oldest state is removed.
        /// </summary>
        public void Record(IState<TContext> state)
        {
            if (_history.Count >= _capacity)
            {
                _history.Dequeue();
                _stateTypes.RemoveAt(0);
            }

            _history.Enqueue(state);
            _stateTypes.Add(state.GetType());
        }

        /// <summary>
        /// Returns a readable string representing the state transition history for debugging purposes only.
        /// </summary>
        public string ToDebugString()
        {
            if (_history.Count == 0) 
            {
                return "<empty>"; 
            }

            StringBuilder sb = new();

            foreach (var state in _history)
            {
                if (sb.Length > 0) sb.Append(" -> ");
                
                sb.Append(state.GetType().Name);
            }

            return sb.ToString();
        }
    }
}
