using R3;

namespace Game.UI.Service
{
    public interface ICombatInputService
    {
        ReadOnlyReactiveProperty<bool> CanNormalAttack { get; }
        ReadOnlyReactiveProperty<float> NormalAttackCooldownPercent { get; }
        void RequestNormalAttack();
    }
}