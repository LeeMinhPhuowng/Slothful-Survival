namespace Game.UI.Data
{
    public readonly struct RunResultPayload
    {
        public readonly string MapId;
        public readonly bool IsWin;
        public readonly int GoldEarned;
        public readonly int EnemyDefeated;
        public readonly float PlayingTimeSeconds;

        public RunResultPayload(string mapId, bool isWin, int goldEarned, int enemyDefeated, float playingTimeSeconds)
        {
            MapId = mapId;
            IsWin = isWin;
            GoldEarned = goldEarned;
            EnemyDefeated = enemyDefeated;
            PlayingTimeSeconds = playingTimeSeconds;
        }
    }
}