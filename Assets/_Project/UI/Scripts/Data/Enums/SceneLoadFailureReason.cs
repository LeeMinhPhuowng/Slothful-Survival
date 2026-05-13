namespace Game.UI.Data
{
    public enum SceneLoadFailureReason
    {
        None,
        SceneNotFound,
        AlreadyLoading,
        MissingGameplayRequest,
        Cancelled,
        Exception
    }
}