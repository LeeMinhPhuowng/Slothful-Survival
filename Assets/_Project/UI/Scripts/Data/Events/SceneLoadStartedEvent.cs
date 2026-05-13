namespace Game.UI.Data
{
    public readonly struct SceneLoadStartedEvent
    {
        public readonly SceneLoadStartedPayload Payload;

        public SceneLoadStartedEvent(SceneLoadStartedPayload payload)
        {
            Payload = payload;
        }
    }
}
