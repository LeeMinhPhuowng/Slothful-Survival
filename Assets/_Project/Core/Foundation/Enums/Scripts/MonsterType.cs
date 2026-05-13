namespace Core.Foundation.Enums
{

    /// <summary>
    /// Types of monsters that can be spawned.
    /// Used by WaveSystem to specify spawn commands and by Combat to instantiate prefabs.
    /// </summary>
    public enum MonsterType
    {
        // Standard enemies (0-99)
        BasicMelee = 0,
        FastRunner = 1,
        Tank = 2,
        Ranged = 3,
        Healer = 4,
        Bomber = 5,

        // Boss enemies (100+)
        Boss = 100,
        MiniBoss = 101
    }
}
