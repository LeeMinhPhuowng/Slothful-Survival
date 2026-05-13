using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public enum InputState
    {
        None,
        Walk,
        Attack, 
        Idle
    }

    public class CubeInputHandler
    {
        private InputState _input;

        public bool IsWalkPressed => _input == InputState.Walk;
        public bool IsAttackPressed => _input == InputState.Attack;
        public bool IsIdlePressed => _input == InputState.Idle;
        
        private readonly float _autoResetDuration = 3f;
        private float _timer = 0f;

        public void HandleKey(KeyCode key)
        {
            switch(key)
            {
                case KeyCode.W:
                    _input = InputState.Walk;
                    break;
                case KeyCode.A:
                    _input = InputState.Attack;
                    break;
                case KeyCode.I:
                    _input = InputState.Idle;
                    break;
            }

            _timer = 0;
        }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= _autoResetDuration)
            {
                ResetInput();
            }
        }

        private void ResetInput()
        {
            _input = InputState.None;
        }
    }
}
