using UnityEngine;

namespace Core.Foundation.FSM.Demo
{

    public class CubeTargetSelector
    {
        public IPosition Target { get; private set; }

        private readonly IPosition _position;
        private readonly IDetector _detector;     
        private readonly float _detectRange;  
        private readonly float _attackRange;

        public CubeTargetSelector(IPosition position, IDetector detector, float detectRange, float attackRange)
        {
            _position = position;
            _detector = detector;
            _detectRange = detectRange;
            _attackRange = attackRange;
        }

        public bool TrySelectTargetInRange()
        {
            if (IsTargetInDetectRange()) return false;
            Target = null;

            float nearestDistance = float.PositiveInfinity;
            var targets = _detector.GetTargetsInRange(_position.Position, _detectRange);

            foreach (var target in targets)
            {
                if (_position.Position.y < target.Position.y + 0.1f) continue;

                float distance = Vector3.Distance(_position.Position, target.Position);
                
                if (distance < nearestDistance)
                {
                    Target = target;
                    nearestDistance = distance;
                }
            }

            return HasTarget;
        }

        public bool HasTarget => Target != null;

        public void ResetTarget()
        {
            Target = null;
        }

        public bool IsTargetInAttackRange()
        {
            return HasTarget && Vector3.Distance(_position.Position, Target.Position) < _attackRange;   
        }

        public bool IsTargetInDetectRange()
        {
            return HasTarget && Vector3.Distance(_position.Position, Target.Position) < _detectRange;   
        }
    }
}
