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
        /// <param name="destinationPath">The destination where the file will be saved.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartDownloadAsync(string url, string destinationPath);

        /// <summary>
        /// Pauses an ongoing download.
        /// </summary>
        /// <param name="downloadId">The ID of the download to pause.</param>
        void PauseDownload(string downloadId);

        /// <summary>
        /// Resumes a paused download.
        /// </summary>
        /// <param name="downloadId">The ID of the download to resume.</param>
        void ResumeDownload(string downloadId);
    }
}
