namespace Game.UI.Presentation.Profile
{
    public readonly struct PlayerStatSummary
    {
        public readonly int MaxHealth;
        public readonly int Damage;
        public readonly int Armor;
        public readonly int MoveSpeed;

        public PlayerStatSummary(int maxHealth, int damage, int armor, int moveSpeed)
        {
            MaxHealth = maxHealth;
            Damage = damage;
            Armor = armor;
            MoveSpeed = moveSpeed;
        }
    }
}
