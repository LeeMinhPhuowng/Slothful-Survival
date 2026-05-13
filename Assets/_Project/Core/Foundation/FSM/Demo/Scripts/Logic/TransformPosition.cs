using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class TransformPosition : IPosition
    {
        private Transform _transform;
        
        public TransformPosition(Transform transform)
        {
            _transform = transform;
        }

        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
    }
}
