namespace Game.UI.Data
{
    public readonly struct SceneLoadFailedEvent
    {
        public readonly SceneLoadFailedPayload Payload;

        public SceneLoadFailedEvent(SceneLoadFailedPayload payload)
        {
            Payload = payload;
        }
    }
}
