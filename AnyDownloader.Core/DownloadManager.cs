using AnyDownloader.Core.Interfaces;

/// <summary>
/// Manages multiple download tasks.
/// </summary>
public class DownloadManager : IDownloadManager
{
    private readonly IHttpDownloader _httpDownloader;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadManager"/> class.
    /// </summary>
    /// <param name="httpDownloader">The HTTP downloader used to download files.</param>
    public DownloadManager(IHttpDownloader httpDownloader)
    {
        _httpDownloader = httpDownloader ?? throw new ArgumentNullException(nameof(httpDownloader));
    }
    /// <summary>
    /// Starts a download asynchronously.
    /// </summary>
    /// <param name="url">The URL of the file to download.</param>
    /// <param name="destinationPath">The destination where the file will be saved.</param>
    public async Task StartDownloadAsync(string url, string destinationPath)
    {
        await _httpDownloader.DownloadFileAsync(url, destinationPath);
    }

    /// <summary>
    /// Pauses a download.
    /// </summary>
    /// <param name="downloadId">The ID of the download to pause.</param>
    public void PauseDownload(string downloadId)
    {
        // Implementation for pausing a download.
    }

    /// <summary>
    /// Resumes a download.
    /// </summary>
    /// <param name="downloadId">The ID of the download to resume.</param>
    public void ResumeDownload(string downloadId)
    {
        // Implementation for resuming a download.
    }
}
