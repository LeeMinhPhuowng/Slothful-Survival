using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class FakeDetector : IDetector
    {
        private readonly List<Vector3> _positions;
        public FakeDetector(List<Vector3> positions)
        {
            _positions = positions;
        }

        public IReadOnlyList<IPosition> GetTargetsInRange(Vector3 position, float detectRange)
        {
            return _positions.Select(p => new FakePosition(p)).ToList();
        }
    }
}
