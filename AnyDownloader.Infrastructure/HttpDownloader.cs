using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnyDownloader.Core.Interfaces;
using AnyDownloader.Models;

namespace AnyDownloader.Infrastructure
{
    /// <summary>
    /// Handles file downloads over HTTP, including file path resolution and progress tracking.
    /// </summary>
    public class HttpDownloader : IHttpDownloader
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpDownloader"/> class.
        /// </summary>
        public HttpDownloader(HttpClient httpClient, IFileSystem fileSystem, ILogger logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<string> ResolveFilePathAsync(string url, string destinationDirectory, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                if (string.IsNullOrEmpty(destinationDirectory))
                {
                    throw new ArgumentNullException(nameof(destinationDirectory), "Destination directory cannot be null or empty.");
                }

                string fileName = ExtractFileName(response, url);
                return Path.Combine(destinationDirectory, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ResolveFilePathAsync] Failed to resolve file path for URL: {url}.", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string url, string destinationPath, Action<DownloadProgress>? progressCallback = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"[DownloadFileAsync] Starting download for URL: {url}");

            string tempFilePath = destinationPath + ".anydownloader";
            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                await DownloadContentToTempFileAsync(response,destinationPath, tempFilePath, progressCallback, cancellationToken);
                _fileSystem.RenameFile(tempFilePath, destinationPath);

                _logger.LogInformation($"[DownloadFileAsync] Successfully downloaded file to: {destinationPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DownloadFileAsync] Error occurred during download process.", ex);
                CleanupTempFile(tempFilePath);
                throw;
            }
        }

        private async Task DownloadContentToTempFileAsync(HttpResponseMessage response,string destinationPath, string tempFilePath, Action<DownloadProgress>? progressCallback, CancellationToken cancellationToken)
        {
            try
            {
                using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var fileStream = _fileSystem.CreateFileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                byte[] buffer = new byte[8192];
                long totalBytesRead = 0;
                long totalBytes = response.Content.Headers.ContentLength ?? -1;

                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    totalBytesRead += bytesRead;

                    progressCallback?.Invoke(new DownloadProgress
                    {
                        FileName = Path.GetFileName(destinationPath),
                        FilePath = destinationPath,
                        TotalBytes = totalBytes,
                        BytesDownloaded = totalBytesRead
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DownloadContentToTempFileAsync] Failed to write content to temp file: {tempFilePath}.", ex);
                throw;
            }
        }

        private void CleanupTempFile(string tempFilePath)
        {
            try
            {
                if (_fileSystem.FileExists(tempFilePath))
                {
                    _fileSystem.DeleteFile(tempFilePath);
                    _logger.LogWarning($"[CleanupTempFile] Temporary file deleted: {tempFilePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[CleanupTempFile] Failed to clean up temporary file: {tempFilePath}.", ex);
            }
        }

        private static string ExtractFileName(HttpResponseMessage response, string url)
        {
            string fileName = response?.Content.Headers.ContentDisposition?.FileNameStar
                              ?? response?.Content.Headers.ContentDisposition?.FileName
                              ?? Path.GetFileName(new Uri(url).LocalPath)
                              ?? "downloadedFile";

            if (fileName.StartsWith("UTF-8''"))
            {
                fileName = WebUtility.UrlDecode(fileName.Substring(7));
            }

            if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
            {
                string contentType = response?.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                fileName = Path.ChangeExtension(fileName, InferFileExtension(contentType));
            }

            return fileName;
        }

        private static string InferFileExtension(string contentType) => contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "application/pdf" => ".pdf",
            "video/mp4" => ".mp4",
            _ => ".bin"
        };
    }
}
