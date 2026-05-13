namespace Game.UI.Data
{
    public enum ReviveFailureReason
    {
        None,
        TimerExpired,
        AdNotReady,
        AdSkipped,
        InsufficientDiamond,
        AlreadyRevived,
        PlayerNotDead,
        Unknown
    }
}