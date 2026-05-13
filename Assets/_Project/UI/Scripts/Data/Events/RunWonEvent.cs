namespace Game.UI.Data
{
    public readonly struct RunWonEvent
    {
        public readonly RunResultPayload Payload;

        public RunWonEvent(RunResultPayload payload)
        {
            Payload = payload;
        }
    }
}
