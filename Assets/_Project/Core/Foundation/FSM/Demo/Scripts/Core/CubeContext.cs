namespace Core.Foundation.FSM.Demo
{

    public class CubeContext
    {
        public ICubeConfig Config { get; }
        public CubeInputHandler Input { get; }
        public CubeHealth Health { get; }
        public CubeMovement Movement { get; }
        public CubeTargetSelector Selector { get; }
        public CubeCombat Combat { get; }


        public CubeContext(IPosition position, IDetector detector, ICubeConfig config)
        {
            Config = config;
            Input = new CubeInputHandler();
            Health = new CubeHealth(config.Health);
            Movement = new CubeMovement(position, config.Speed);
            Selector = new CubeTargetSelector(position, detector, config.DetectRange, config.AttackRange);
            Combat = new CubeCombat(config.AttackCooldown, config.AttackDuration);
        }
    }
}
