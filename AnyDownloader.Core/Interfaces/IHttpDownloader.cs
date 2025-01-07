using AnyDownloader.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyDownloader.Core.Interfaces
{
    /// <summary>
    /// Interface for downloading files over HTTP.
    /// </summary>
    public interface IHttpDownloader
    {
        /// <summary>
        /// Downloads a file asynchronously from the specified URL to the given destination.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="destination">The destination path where the file will be saved.</param>
        /// <param name="progressCallback">
        /// Optional callback for reporting download progress. Provides details such as file size, bytes downloaded, speed, and estimated time remaining.
        /// </param>
        /// <param name="cancellationToken">A token to cancel the download operation, default is no cancellation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DownloadFileAsync(
            string url,
            string destination,
            Action<DownloadProgress>? progressCallback = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves the full file path by making an HTTP request for headers.
        /// </summary>
        /// <param name="url">The URL of the file.</param>
        /// <param name="destinationDirectory">The directory where the file will be saved.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resolved file path, including the file name and extension.</returns>
        Task<string> ResolveFilePathAsync(string url, string destinationDirectory, CancellationToken cancellationToken = default);
    }
}
