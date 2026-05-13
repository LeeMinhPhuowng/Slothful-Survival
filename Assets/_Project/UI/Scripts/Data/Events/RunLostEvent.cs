namespace Game.UI.Data
{
    public readonly struct RunLostEvent
    {
        public readonly RunResultPayload Payload;

        public RunLostEvent(RunResultPayload payload)
        {
            Payload = payload;
        }
    }
}
