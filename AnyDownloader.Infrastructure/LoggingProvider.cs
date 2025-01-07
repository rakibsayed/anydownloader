using System;
using Serilog;

namespace AnyDownloader.Infrastructure
{
    /// <summary>
    /// Provides logging functionality using Serilog.
    /// Implements the ILogger interface for dependency injection.
    /// </summary>
    public class LoggingProvider : AnyDownloader.Core.Interfaces.ILogger
    {
        private readonly Serilog.ILogger _logger;

        public LoggingProvider()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void LogInformation(string message) => _logger.Information(message);
        public void LogWarning(string message) => _logger.Warning(message);
        public void LogError(string message, Exception? ex = null) => _logger.Error(ex, message);
    }
}