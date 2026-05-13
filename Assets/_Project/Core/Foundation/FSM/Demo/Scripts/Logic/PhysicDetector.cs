using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class PhysicDetector : IDetector
    {
        public IReadOnlyList<IPosition> GetTargetsInRange(Vector3 position, float detectRange)
        {
            var hits = Physics2D.OverlapCircleAll(position, detectRange);
            return hits.Select(hit => new TransformPosition(hit.transform)).ToList();
        }
    }
}
