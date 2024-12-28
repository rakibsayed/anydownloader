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
        /// <param name="destination">The file path where the downloaded file will be saved.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DownloadFileAsync(string url, string destination);
    }
}
