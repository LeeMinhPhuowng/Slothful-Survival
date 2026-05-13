using UnityEngine;

namespace Core.Foundation.Events.Demo
{

    public class CubeColorService
    {
        public Color RandomColor()
        {
            return Random.ColorHSV();
        }
    }
}
