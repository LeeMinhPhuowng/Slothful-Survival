using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class CubeMovement
    {
        private readonly IPosition _position;
        private readonly float _speed;

        public CubeMovement(IPosition position, float speed)
        {
            _position = position;
            _speed = speed;
        }

        public void MoveToward(Vector3 target, float deltaTime)
        {
            Vector3 direction = (target - _position.Position).normalized;
            MoveDirection(direction, deltaTime);
        }

        public void MoveDirection(Vector3 direction, float deltaTime)
        {
            _position.Position += _speed * deltaTime * direction;
        }
    }
}
