namespace Game.UI.Data
{
    public readonly struct SceneLoadCompletedEvent
    {
        public readonly SceneLoadCompletedPayload Payload;

        public SceneLoadCompletedEvent(SceneLoadCompletedPayload payload)
        {
            Payload = payload;
        }
    }
}
