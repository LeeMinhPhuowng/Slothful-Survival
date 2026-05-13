namespace Core.Foundation.Logging
{

    /// <summary>
    /// Static facade over a default <see cref="LoggerFactory"/>.
    /// DI-aware code can [Inject] LoggerFactory directly; legacy callsites use this static facade.
    /// Bootstrap calls <see cref="Configure(LogConfigSO)"/> to apply runtime config.
    /// </summary>
    public static class LogManager
    {
        public static LoggerFactory Default { get; } = new LoggerFactory();

        public static ILogger GetLogger<T>() => Default.GetLogger<T>();

        public static ILogger GetLogger(string category) => Default.GetLogger(category);

        public static ILogger GetLogger(object source) =>
            source != null ? Default.GetLogger(source.GetType().FullName) : Default.GetLogger("Null");

        public static void Configure(LogConfigSO so) => Default.Configure(so);

        public static void Configure(LogConfig config) => Default.Configure(config);
    }
}
