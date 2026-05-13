namespace Game.UI.Data
{
    public readonly struct GameplayLoadRequest
    {
        public readonly string MapId;
        public readonly string CharacterId;
        public readonly bool IsRetry;

        public GameplayLoadRequest(string mapId, string characterId, bool isRetry = false)
        {
            MapId = mapId;
            CharacterId = characterId;
            IsRetry = isRetry;
        }
    }
}