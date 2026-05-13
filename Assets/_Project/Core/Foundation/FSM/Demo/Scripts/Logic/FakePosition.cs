using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class FakePosition : IPosition
    {
        public Vector3 Position { get; set; }

        public FakePosition(Vector3 position)
        {
            Position = position;
        }
    }
}
