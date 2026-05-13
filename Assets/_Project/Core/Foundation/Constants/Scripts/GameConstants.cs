namespace Core.Foundation.Constants
{

    /// <summary>
    /// Global constants shared across all features.
    /// </summary>
    public static class GameConstants
    {
        // Lane System (WaveSystem, Combat)
        public const int LaneCount = 5;
        public const int RandomLaneIndex = -1;

        // Spawn Limits (WaveSystem validation)
        public const int MaxSpawnCountPerGroup = 50;
        public const int MaxGroupsPerWave = 20;
        public const int MaxWavesPerLevel = 100;

        // Economy Defaults
        public const int DefaultEnergySoftCap = 20;
        public const int DefaultEnergyHardCap = 999;
        public const float DefaultEnergyRegenSeconds = 60f;

        // Combat Defaults
        public const int MaxEntitiesPerScene = 200;

        // Gacha Merge Defaults
        public const int MaxQueueSize = 7;
        public const float EnergyResponseTimeout = 5f;
    }
}
