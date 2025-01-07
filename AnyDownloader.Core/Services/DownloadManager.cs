using System.Collections.Concurrent;
using AnyDownloader.Core.Interfaces;
using AnyDownloader.Models;

namespace AnyDownloader.Core.Services
{
    /// <summary>
    /// Manages multiple download tasks and handles their lifecycle.
    /// </summary>
    public class DownloadManager : IDownloadManager
    {
        private readonly IHttpDownloader _httpDownloader;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, DownloadTask> _activeDownloads;
        private readonly string _defaultDownloadFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadManager"/> class.
        /// </summary>
        /// <param name="httpDownloader">The HTTP downloader instance for managing file downloads.</param>
        /// <param name="logger">The logger instance for logging download events and errors.</param>
        public DownloadManager(IHttpDownloader httpDownloader, ILogger logger)
        {
            _httpDownloader = httpDownloader ?? throw new ArgumentNullException(nameof(httpDownloader));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activeDownloads = new ConcurrentDictionary<string, DownloadTask>();

            // Default to the user's Downloads folder.
            _defaultDownloadFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");
        }

        /// <inheritdoc />
        public async Task StartDownloadAsync(string url, string? destinationDirectory = null, Action<DownloadProgress>? progressCallback = null)
        {
            ValidateUrl(url);
            destinationDirectory ??= _defaultDownloadFolder;

            EnsureDirectoryExists(destinationDirectory);

            // Resolve the file path
            string resolvedFilePath = await _httpDownloader.ResolveFilePathAsync(url, destinationDirectory);
            _logger.LogInformation($"[StartDownloadAsync] Resolved file path: {resolvedFilePath}");

            string downloadId = Guid.NewGuid().ToString();
            var cancellationTokenSource = new CancellationTokenSource();
            var downloadTask = new DownloadTask(url, resolvedFilePath, cancellationTokenSource);

            if (!_activeDownloads.TryAdd(downloadId, downloadTask))
            {
                _logger.LogError($"[StartDownloadAsync] Failed to add download task for URL: {url}");
                throw new InvalidOperationException("Failed to add download task.");
            }

            try
            {
                _logger.LogInformation($"[StartDownloadAsync] Starting download task for {resolvedFilePath}");
                await _httpDownloader.DownloadFileAsync(url, resolvedFilePath, progressCallback, cancellationTokenSource.Token);
                _logger.LogInformation($"[StartDownloadAsync] Download completed for {resolvedFilePath}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"[StartDownloadAsync] Download task for {resolvedFilePath} was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StartDownloadAsync] Error during download task for {resolvedFilePath}: {ex.Message}", ex);
                throw;
            }
            finally
            {
                _activeDownloads.TryRemove(downloadId, out _);
            }
        }

        /// <inheritdoc />
        public void PauseDownload(string downloadId)
        {
            if (_activeDownloads.TryGetValue(downloadId, out var downloadTask))
            {
                downloadTask.CancellationTokenSource.Cancel();
                _logger.LogInformation($"[PauseDownload] Download {downloadId} has been paused.");
            }
            else
            {
                LogAndThrowDownloadNotFound(downloadId, nameof(PauseDownload));
            }
        }

        /// <inheritdoc />
        public void ResumeDownload(string downloadId)
        {
            if (_activeDownloads.TryGetValue(downloadId, out var downloadTask))
            {
                if (!File.Exists(downloadTask.FilePath))
                {
                    _logger.LogError($"[ResumeDownload] Cannot resume download. File at {downloadTask.FilePath} does not exist.");
                    throw new InvalidOperationException($"File at {downloadTask.FilePath} does not exist.");
                }

                _logger.LogInformation($"[ResumeDownload] Resuming download {downloadId}...");
                StartDownloadAsync(downloadTask.Url, Path.GetDirectoryName(downloadTask.FilePath)).Wait();
            }
            else
            {
                LogAndThrowDownloadNotFound(downloadId, nameof(ResumeDownload));
            }
        }

        /// <summary>
        /// Validates the URL and ensures it uses a supported scheme.
        /// </summary>
        /// <param name="url">The URL to validate.</param>
        /// <exception cref="NotSupportedException">Thrown when the URL scheme is not HTTP or HTTPS.</exception>
        private void ValidateUrl(string url)
        {
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError($"[ValidateUrl] Unsupported URL scheme: {url}");
                throw new NotSupportedException($"The '{new Uri(url).Scheme}' scheme is not supported.");
            }
        }

        /// <summary>
        /// Ensures the specified directory exists, creating it if necessary.
        /// </summary>
        /// <param name="directory">The directory path to check or create.</param>
        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation($"[EnsureDirectoryExists] Created directory: {directory}");
            }
        }

        /// <summary>
        /// Logs an error for a missing download and throws an exception.
        /// </summary>
        /// <param name="downloadId">The ID of the missing download.</param>
        /// <param name="operation">The operation being performed (e.g., "PauseDownload").</param>
        /// <exception cref="ArgumentException">Always thrown with the specified download ID and operation.</exception>
        private void LogAndThrowDownloadNotFound(string downloadId, string operation)
        {
            _logger.LogError($"[{operation}] No active download found with ID: {downloadId}");
            throw new ArgumentException($"No active download found with ID: {downloadId}", nameof(downloadId));
        }
    }
}
