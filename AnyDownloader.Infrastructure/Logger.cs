using Serilog;

namespace AnyDownloader.Infrastructure
{
    /// <summary>
    /// Configures application-wide logging.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Configures Serilog for the application.
        /// </summary>
        public static void Configure()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
