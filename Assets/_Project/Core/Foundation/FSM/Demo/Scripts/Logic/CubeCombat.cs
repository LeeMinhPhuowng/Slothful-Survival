namespace Core.Foundation.FSM.Demo
{

    public class CubeCombat
    {
        private readonly float _attackCooldown;
        private readonly float _attackDuration;

        private float _cooldownTimer;
        private float _durationTimer;

        private bool isAttacking;

        public CubeCombat(float attackCooldown, float attackDuration)
        {
            _attackCooldown = attackCooldown;
            _attackDuration = attackDuration;

            isAttacking = false;
        }

        public bool IsCooldownFinished => _cooldownTimer < 0;
        public bool IsAttackFinished => _durationTimer < 0;
        
        public void Tick(float deltaTime)
        {
            if (isAttacking)
            {
                _durationTimer -= deltaTime;
            }
            else _cooldownTimer -= deltaTime;
        }

        public void StartAttackDuration()
        {
            isAttacking = true;
            _durationTimer = _attackDuration;
        }

        public void StartCooldown()
        {
            isAttacking = false;
            _cooldownTimer = _attackCooldown;
        }
    }
}
