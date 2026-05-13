namespace Core.Foundation.FSM.Demo
{

    public interface ICubeConfig
    {
        int Health { get; }
        float Speed { get; }
        float DetectRange { get; }
        float AttackRange { get; }
        float AttackCooldown { get; }
        float AttackDuration { get; }
    }
}
