using AnyDownloader.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Unit tests for the HttpDownloader class.
/// Verifies the functionality of the HttpDownloader's methods for downloading files.
/// </summary>
public class HttpDownloaderTests
{
    /// <summary>
    /// Verifies that DownloadFileAsync successfully downloads a file to the specified destination.
    /// </summary>
    [Fact]
    public async Task DownloadFileAsync_ShouldDownloadFileSuccessfully()
    {
        // Arrange: Create an instance of HttpDownloader and define a valid URL and destination path.
        var httpDownloader = new HttpDownloader();
        string url = "https://images.unsplash.com/photo-1721332154191-ba5f1534266e?q=80&w=735&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDF8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"; // Replace with a valid URL
        string destinationPath = Path.Combine(Path.GetTempPath(), "http_downloader_testfile.jpg");

        // Ensure directory exists.
        var directoryPath = Path.GetDirectoryName(destinationPath);
        if (directoryPath != null)
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Clean up any existing file.
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        // Act: Download the file.
        await httpDownloader.DownloadFileAsync(url, destinationPath);

        // Assert: Verify that the file was downloaded successfully.
        Assert.True(File.Exists(destinationPath), $"The file should have been downloaded to {destinationPath}.");


        // Cleanup: Remove the downloaded file.
        File.Delete(destinationPath);
        Assert.False(File.Exists(destinationPath), $"The file at {destinationPath} should have been deleted.");
    }

    /// <summary>
    /// Verifies that DownloadFileAsync throws an exception when provided with an invalid URL.
    /// </summary>
    [Fact]
    public async Task DownloadFileAsync_ShouldThrowException_ForInvalidUrl()
    {
        // Arrange: Create an instance of HttpDownloader and define an invalid URL.
        var httpDownloader = new HttpDownloader();
        string invalidUrl = "invalid-url"; // Malformed URL
        string destinationPath = Path.Combine(Path.GetTempPath(), "invalid_testfile.txt");

        // Act & Assert: Verify that an ArgumentException is thrown for the invalid URL.
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            httpDownloader.DownloadFileAsync(invalidUrl, destinationPath));

        // Verify the exception message for clarity.
        Assert.Equal($"Invalid URL: {invalidUrl}", exception.Message);

    }
}
