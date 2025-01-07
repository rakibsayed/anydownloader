namespace AnyDownloader.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for logging messages with different levels of severity.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an informational message, typically for general runtime events.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        void LogInformation(string message);

        /// <summary>
        /// Logs a warning message, typically for non-critical issues or unexpected behavior.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        void LogWarning(string message);

        /// <summary>
        /// Logs an error message, typically for critical issues or exceptions.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="ex">The exception associated with the error, if any. Null by default.</param>
        void LogError(string message, Exception? ex = null);
    }
}
