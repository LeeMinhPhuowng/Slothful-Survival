using System;
using System.Collections.Generic;
using System.Text;
using Core.Foundation.Logging;

namespace Core.Foundation.FSM
{

    /// <summary>
    /// Implements a generic finite state machine.
    /// Manages state lifecycle, transitions, and transition history.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context object shared across all states, providing data and services
    /// required during state execution.
    /// </typeparam>
    public class StateMachine<TContext> : IStateMachine<TContext>
    {
        /// <summary>
        /// Gets the currently active state.
        /// </summary>
        public IState<TContext> CurrentState { get; private set; }

        /// <summary>
        /// Gets the state transition history.
        /// </summary>
        public StateHistory<TContext> History { get; }
        
        /// <summary>
        /// Raised when a state has been entered.
        /// </summary>
        public event Action<IState<TContext>> OnStateEntered;

        /// <summary>
        /// Raised when a state has been exited.
        /// </summary>
        public event Action<IState<TContext>> OnStateExited;
        
        private readonly TContext _context;
        private readonly List<StateTransition<TContext>> _transitions = new();
        private readonly ILogger _logger;
        

        /// <summary>
        ///  Creates a new state machine instance.
        /// </summary> 
        public StateMachine(TContext context, int historyCapacity)
        {   
            History = new StateHistory<TContext>(historyCapacity);
            OnStateEntered += History.Record;

            _context = context;

            _logger = LogManager.GetLogger<StateMachine<TContext>>();
            _logger.Info($"FSM created with context type: {typeof(TContext).Name}");
        }

        /// <summary>
        /// Initializes the state machine with an initial state.
        /// </summary>
        public void Initialize(IState<TContext> initialState)
        {
            CurrentState = initialState;

            CurrentState.Enter(_context);
            OnStateEntered?.Invoke(initialState);
            
            _logger.Info($"FSM initialized with state: {initialState?.GetType().Name}");
        }

        /// <summary>
        /// Transitions the state machine to a new state.
        /// </summary>
        public void ChangeState(IState<TContext> newState)
        {   
            if (newState == null)
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Error($"State change ignored: attempted to transition from {CurrentState?.GetType().Name} to <null>");
                }
                return;
            }

            if (CurrentState == newState) 
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug($"State change ignored: CurrentState already in {newState.GetType().Name}");
                }
                return;
            }

            _logger.Info($"State change success: {CurrentState.GetType().Name} -> {newState.GetType().Name}");
            
            CurrentState.Exit(_context);
            OnStateExited?.Invoke(CurrentState);

            CurrentState = newState;
            
            CurrentState.Enter(_context);
            OnStateEntered?.Invoke(CurrentState);
            
            if (_logger.IsDebugEnabled)
            {
                _logger.Info($"State transition history: {History.ToDebugString()}");
            }
        }

        /// <summary>
        /// Updates the current state and evaluates transitions every frame.
        /// </summary>
        public void Update(float deltaTime)
        {
            CurrentState?.Update(_context, deltaTime);
            TryTransition();
        }

        /// <summary>
        /// Updates the current state at a fixed time step.
        /// </summary>
        public void FixedUpdate(float fixedDeltaTime)
        {
            CurrentState?.FixedUpdate(_context, fixedDeltaTime);
        }

        /// <summary>
        /// Registers a transition between two states with optional guards.
        /// </summary>
        public void AddTransition(IState<TContext> from, IState<TContext> to, params ITransitionGuard<TContext>[] guards)
        {
            _transitions.Add(new StateTransition<TContext>(from, to, guards));
        }

        private void TryTransition()
        {
            if (CurrentState == null)
            {
                _logger.Error("Cannot evaluate transitions because CurrentState is null");
                return;
            }

            foreach (var transition in _transitions)
            {
                // A null source state represents a global transition (valid from Any State)
                if (transition.From == null || transition.From == CurrentState) 
                {
                    if (transition.CanTransition(_context)) 
                    {
                        ChangeState(transition.To);
                        break;
                    }
                }
            }
        }


        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only helper that returns a formatted debug string
        /// describing all registered transitions and their guard evaluations.
        /// </summary>
        public string GetTransitionsDebugString()
        {
            if (_transitions == null || _transitions.Count == 0)
            {
                return "<No Transitions>";
            }

            StringBuilder sb = new();

            foreach (var transition in _transitions)
            {
                sb.AppendLine(transition.ToDebugString(_context));
            }

            return sb.ToString();
        }
        #endif
    }
}
