namespace Core.Foundation.FSM.Demo
{

    public class FakeCubeConfig : ICubeConfig
    {
        public int Health { get; }
        public float Speed { get; }
        public float DetectRange { get; }
        public float AttackRange { get; }
        public float AttackCooldown { get; }
        public float AttackDuration { get; }

        public FakeCubeConfig(int health = 1, float speed = 1, float detectRange = 4, float attackRange = 1f, 
            float attackCooldown = 2, float attackDuration = 5)
        {
            Health = health;
            Speed = speed;
            DetectRange = detectRange;
            AttackRange = attackRange;
            AttackCooldown = attackCooldown;
            AttackDuration = attackDuration;
        }
    }
}
