using AnyDownloader.Models;

namespace AnyDownloader.Core.Interfaces
{
    /// <summary>
    /// Interface for managing download operations.
    /// </summary>
    public interface IDownloadManager
    {
        /// <summary>
        /// Starts a download asynchronously.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="destinationPath">
        /// The directory where the file will be saved. If null, the file will be saved in the default download folder.
        /// If the path includes a file name, it will be used; otherwise, the file name will be resolved automatically.
        /// </param>
        /// <param name="progressCallback">
        /// Optional callback for reporting download progress. Provides details such as file size, bytes downloaded, speed, and estimated time remaining.
        /// </param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartDownloadAsync(string url, string? destinationPath = null, Action<DownloadProgress>? progressCallback = null);

        /// <summary>
        /// Pauses an ongoing download.
        /// </summary>
        /// <param name="downloadId">The unique identifier of the download to pause.</param>
        void PauseDownload(string downloadId);

        /// <summary>
        /// Resumes a paused download.
        /// </summary>
        /// <param name="downloadId">The unique identifier of the download to resume.</param>
        void ResumeDownload(string downloadId);
    }
}
