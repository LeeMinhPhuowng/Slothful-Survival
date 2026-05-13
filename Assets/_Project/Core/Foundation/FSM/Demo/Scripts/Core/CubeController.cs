using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Foundation.FSM.Demo
{

    [RequireComponent(typeof(CubeView))]
    public class CubeController : MonoBehaviour, IFsmDebugView
    {
        [SerializeField] private CubeConfigSO _config;
        private StateMachine<CubeContext> _stateMachine;
        private CubeContext _context;

        private readonly WalkState _walkState = new();
        private readonly ChaseState _chaseState = new();
        private readonly AttackState _attackState = new();
        private readonly IdleState _idleState = new();
        private readonly DieState _dieState = new();

        private event Action<KeyCode> OnKeyPressed;


        public GameObject Owner => gameObject;
        public Type ContextType => _context.GetType();
        public IReadOnlyCollection<Type> History => _stateMachine.History.StateTypes;
        public string Transitions => _stateMachine.GetTransitionsDebugString();


        private void Awake()
        {
            _context = new(new TransformPosition(transform), new PhysicDetector(), _config);
            _stateMachine = StateMachineFactory.Create(_context, _walkState, 5);

            var view = GetComponent<CubeView>();
            view.Bind(_stateMachine);

            CubeFSMTransitionBuilder.Build(_stateMachine, _walkState, _chaseState, _attackState, _idleState, _dieState);
        }

        private void OnEnable()
        {
            OnKeyPressed += _context.Input.HandleKey;
            FSMDebugRegistry.Register(this);
        }

        private void OnDisable()
        {
            OnKeyPressed -= _context.Input.HandleKey;
            FSMDebugRegistry.UnRegister(this);
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
            HandleKeyBoard();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate(Time.fixedDeltaTime);
        }

        private void HandleKeyBoard()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.wKey.wasPressedThisFrame)
            {
                OnKeyPressed?.Invoke(KeyCode.W);
            }

            if (keyboard.aKey.wasPressedThisFrame)
            {
                OnKeyPressed?.Invoke(KeyCode.A);
            }
                
            if (keyboard.iKey.wasPressedThisFrame)
            {
                OnKeyPressed?.Invoke(KeyCode.I);
            }
        }
    }
}
