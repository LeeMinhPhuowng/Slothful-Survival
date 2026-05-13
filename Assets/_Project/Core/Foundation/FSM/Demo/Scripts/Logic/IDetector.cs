using System.Collections.Generic;
using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public interface IDetector
    {
        IReadOnlyList<IPosition> GetTargetsInRange(Vector3 position, float detectRange);
    }
}
