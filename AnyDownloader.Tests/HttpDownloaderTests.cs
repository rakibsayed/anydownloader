using AnyDownloader.Infrastructure;
using AnyDownloader.Core.Interfaces;
using NSubstitute;
using Xunit;

namespace AnyDownloader.Tests
{
    /// <summary>
    /// Unit tests for the HttpDownloader class.
    /// </summary>
    public class HttpDownloaderTests
    {
        private readonly ILogger _loggerMock;
        private readonly IFileSystem _fileSystemMock;

        public HttpDownloaderTests()
        {
            _loggerMock = Substitute.For<ILogger>();
            _fileSystemMock = Substitute.For<IFileSystem>();
        }

        /// <summary>
        /// Custom HttpMessageHandler for simulating HTTP responses.
        /// </summary>
        public class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage? _response;
            private readonly Exception? _exceptionToThrow;

            public MockHttpMessageHandler(HttpResponseMessage response)
            {
                _response = response ?? throw new ArgumentNullException(nameof(response));
            }

            public MockHttpMessageHandler(Exception exceptionToThrow)
            {
                _exceptionToThrow = exceptionToThrow ?? throw new ArgumentNullException(nameof(exceptionToThrow));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_exceptionToThrow != null)
                {
                    throw _exceptionToThrow;
                }

                return Task.FromResult(_response ?? new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
            }
        }

        [Fact]
        public async Task ResolveFilePathAsync_ShouldResolvePath_WithCorrectFileNameAndExtension()
        {
            // Arrange
            string url = "https://fake-url.com/sample";
            string destinationFolder = Path.GetTempPath();

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("Mock Content")
            };
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "mockedfile.jpg"
            };

            var mockHandler = new MockHttpMessageHandler(response);
            var httpClient = new HttpClient(mockHandler);
            var downloader = new HttpDownloader(httpClient, _fileSystemMock, _loggerMock);

            // Act
            var resolvedPath = await downloader.ResolveFilePathAsync(url, destinationFolder);

            // Assert
            Assert.Equal(Path.Combine(destinationFolder, "mockedfile.jpg"), resolvedPath);
        }

        [Fact]
        public async Task DownloadFileAsync_ShouldLogError_OnDownloadFailure()
        {
            // Arrange
            string invalidUrl = "https://fake-url.com/nonexistentfile";
            string destination = Path.Combine(Path.GetTempPath(), "nonexistentfile.txt");
            string tempFilePath = destination + ".anydownloader";

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            var mockHandler = new MockHttpMessageHandler(response);
            var httpClient = new HttpClient(mockHandler);
            var downloader = new HttpDownloader(httpClient, _fileSystemMock, _loggerMock);

            // Mock file system behavior
            _fileSystemMock.FileExists(tempFilePath).Returns(true);
            _fileSystemMock.When(fs => fs.DeleteFile(tempFilePath)).Do(_ => { });

            // Act
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => downloader.DownloadFileAsync(invalidUrl, destination));
            Assert.Contains("Response status code does not indicate success", exception.Message);

            // Verify cleanup
            _fileSystemMock.Received(1).DeleteFile(tempFilePath);

            // Verify logging
            _loggerMock.Received(1).LogError(Arg.Is<string>(msg => msg.Contains("[DownloadFileAsync] Error occurred during download process.")), Arg.Any<Exception>());
        }

        [Fact]
        public async Task DownloadFileAsync_ShouldDetectExtension_AndSaveFileCorrectly()
        {
            // Arrange
            string validUrl = "https://fake-url.com/sample.txt";
            string destinationFolder = Path.GetTempPath();
            string tempFilePath = Path.Combine(destinationFolder, "sample.txt.anydownloader");
            string destinationPath = Path.Combine(destinationFolder, "sample.txt");

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Mock Content")))
            };
            var mockHandler = new MockHttpMessageHandler(response);
            var httpClient = new HttpClient(mockHandler);
            var downloader = new HttpDownloader(httpClient, _fileSystemMock, _loggerMock);

            // Mock file system behavior
            _fileSystemMock.CreateFileStream(
                tempFilePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None)
                .Returns(callInfo => File.Create(tempFilePath));

            _fileSystemMock.FileExists(tempFilePath).Returns(callInfo => File.Exists(tempFilePath));
            _fileSystemMock.FileExists(destinationPath).Returns(callInfo => File.Exists(destinationPath));

            // Act
            await downloader.DownloadFileAsync(validUrl, destinationPath);

            // Assert
            _fileSystemMock.Received(1).CreateFileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            _fileSystemMock.Received(1).RenameFile(tempFilePath, destinationPath);
            _loggerMock.Received(1).LogInformation(Arg.Is<string>(msg => msg.Contains($"[DownloadFileAsync] Successfully downloaded file to: {destinationPath}")));

            // Cleanup
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
        }

        [Fact]
        public async Task DownloadFileAsync_ShouldCleanUpTemporaryFile_OnError()
        {
            // Arrange
            string url = "https://fake-url.com/sample.txt";
            string destinationFolder = Path.GetTempPath();
            string destinationPath = Path.Combine(destinationFolder, "sample.txt");
            string tempFilePath = Path.Combine(destinationFolder, "sample.txt.anydownloader");

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            var mockHandler = new MockHttpMessageHandler(response);
            var httpClient = new HttpClient(mockHandler);
            var downloader = new HttpDownloader(httpClient, _fileSystemMock, _loggerMock);

            // Mock file system behavior
            _fileSystemMock.FileExists(tempFilePath).Returns(true);
            _fileSystemMock.When(fs => fs.DeleteFile(tempFilePath)).Do(_ => { });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => downloader.DownloadFileAsync(url, destinationPath));
            Assert.Contains("Response status code does not indicate success", exception.Message);

            // Verify cleanup
            _fileSystemMock.Received(1).DeleteFile(tempFilePath);
            _loggerMock.Received(1).LogError(
                Arg.Is<string>(msg => msg.Contains("[DownloadFileAsync] Error occurred during download process.")),
                Arg.Any<Exception>());

        }
    }
}
