using System;

namespace Game.UI.Data
{
    public static class GameplayRunSignals
    {
        public static event Action EnemyKilled;
        public static event Action PlayerDied;

        public static void ReportEnemyKilled()
        {
            EnemyKilled?.Invoke();
        }

        public static void ReportPlayerDied()
        {
            PlayerDied?.Invoke();
        }

        public static void Clear()
        {
            EnemyKilled = null;
            PlayerDied = null;
        }
    }
}
