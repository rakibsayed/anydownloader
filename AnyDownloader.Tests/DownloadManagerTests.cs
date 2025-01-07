using AnyDownloader.Core.Services;
using AnyDownloader.Core.Interfaces;
using AnyDownloader.Models;
using NSubstitute;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnyDownloader.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="DownloadManager"/> class.
    /// </summary>
    public class DownloadManagerTests
    {
        private readonly IHttpDownloader _httpDownloaderMock;
        private readonly ILogger _loggerMock;
        private readonly DownloadManager _downloadManager;

        public DownloadManagerTests()
        {
            // Initialize mocks and the DownloadManager instance
            _httpDownloaderMock = Substitute.For<IHttpDownloader>();
            _loggerMock = Substitute.For<ILogger>();
            _downloadManager = new DownloadManager(_httpDownloaderMock, _loggerMock);
        }

        [Fact]
        public async Task StartDownloadAsync_ShouldCallHttpDownloader_AndCreateFile()
        {
            // Arrange
            string url = "https://example.com/sample.jpg";
            string destinationDirectory = Path.GetTempPath();
            string resolvedFilePath = Path.Combine(destinationDirectory, "sample.jpg");

            // Mock ResolveFilePathAsync to return a resolved file path
            _httpDownloaderMock.ResolveFilePathAsync(url, destinationDirectory, Arg.Any<CancellationToken>())
                               .Returns(Task.FromResult(resolvedFilePath));

            // Mock DownloadFileAsync to simulate file download
            _httpDownloaderMock.When(d =>
                d.DownloadFileAsync(url, resolvedFilePath, Arg.Any<Action<DownloadProgress>>(), Arg.Any<CancellationToken>()))
                               .Do(_ => File.WriteAllText(resolvedFilePath, "Mock content"));

            // Act
            await _downloadManager.StartDownloadAsync(url, destinationDirectory);

            // Assert
            await _httpDownloaderMock.Received(1).ResolveFilePathAsync(url, destinationDirectory, Arg.Any<CancellationToken>());
            await _httpDownloaderMock.Received(1).DownloadFileAsync(url, resolvedFilePath, Arg.Any<Action<DownloadProgress>>(), Arg.Any<CancellationToken>());

            // Verify that the file was created
            Assert.True(File.Exists(resolvedFilePath), "The file should exist after the download is completed.");

            // Cleanup
            File.Delete(resolvedFilePath);
        }

        [Fact]
        public async Task StartDownloadAsync_ShouldThrowException_WhenUrlIsInvalid()
        {
            // Arrange
            string invalidUrl = "ftp://example.com/sample.jpg"; // Unsupported URL scheme
            string destinationDirectory = Path.GetTempPath();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotSupportedException>(() => _downloadManager.StartDownloadAsync(invalidUrl, destinationDirectory));
            Assert.Equal("The 'ftp' scheme is not supported.", exception.Message);

            // Verify that no download was attempted
            await _httpDownloaderMock.DidNotReceiveWithAnyArgs().DownloadFileAsync(invalidUrl,destinationDirectory , default, default);
        }
    }
}
