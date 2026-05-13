using UnityEngine;

namespace Core.Foundation.Events.Demo
{

    public class CubeVerticalMovement
    {
        public Vector3 MoveUp(Vector3 position, int step)
        {
            return position + Vector3.up *  step;
        }

        public Vector3 MoveDown(Vector3 position, int step)
        {
            return position + Vector3.down * step;
        }
    }
}
