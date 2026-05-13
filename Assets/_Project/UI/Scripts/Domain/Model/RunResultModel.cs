using System;

namespace Game.UI.Model
{
    public sealed class RunResultModel
    {
        public int GoldEarned;
        public int EnemyDefeated;
        public TimeSpan PlayingTime;
        public bool IsWin;
    }
}