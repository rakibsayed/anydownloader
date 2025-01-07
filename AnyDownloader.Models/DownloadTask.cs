using System;
using System.Threading;

namespace AnyDownloader.Models
{
    /// <summary>
    /// Represents an individual download task with its associated metadata.
    /// </summary>
    public class DownloadTask
    {
        /// <summary>
        /// Gets the URL of the file being downloaded.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the full file path where the file will be saved.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the cancellation token source for managing task cancellation.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadTask"/> class.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="filePath">The full file path where the file will be saved.</param>
        /// <param name="cancellationTokenSource">The cancellation token source for the task.</param>
        public DownloadTask(string url, string filePath, CancellationTokenSource cancellationTokenSource)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            CancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        /// <summary>
        /// Returns a string representation of the download task.
        /// </summary>
        /// <returns>A string containing the URL and file path.</returns>
        public override string ToString() => $"DownloadTask: URL={Url}, FilePath={FilePath}";
    }
}
