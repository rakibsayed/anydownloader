using AnyDownloader.Core;
using AnyDownloader.Core.Interfaces;
using NSubstitute;
using System.IO;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Unit tests for the DownloadManager class.
/// Verifies the functionality of the DownloadManager's methods using a mocked IHttpDownloader.
/// </summary>
public class DownloadManagerTests
{
    /// <summary>
    /// Verifies that StartDownloadAsync calls the DownloadFileAsync method on IHttpDownloader
    /// and that the file is created at the specified destination.
    /// </summary>
    [Fact]
    public async Task StartDownloadAsync_ShouldCallHttpDownloader_AndCreateFile()
    {
        // Arrange: Mock the IHttpDownloader and create a DownloadManager instance.
        var httpDownloader = Substitute.For<IHttpDownloader>();
        var downloadManager = new DownloadManager(httpDownloader);

        string url = "https://images.unsplash.com/photo-1721332154191-ba5f1534266e?q=80&w=735&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDF8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"; 
        // Replace with a valid URL
        string destinationPath = Path.Combine(Path.GetTempPath(), "download_manager_testfile.jpg");

        // Simulate file creation for the mock.
        httpDownloader.When(d => d.DownloadFileAsync(url, destinationPath))
            .Do(_ => File.WriteAllText(destinationPath, "Mock file content"));

        // Act: Start the download process.
        await downloadManager.StartDownloadAsync(url, destinationPath);

        // Assert: Verify that the file was created and the mock was called.
        Assert.True(File.Exists(destinationPath), $"The file should have been downloaded to {destinationPath}.");

        // Cleanup: Remove the test file.
        File.Delete(destinationPath);
        Assert.False(File.Exists(destinationPath), $"The file at {destinationPath} should have been deleted.");
    }

    /// <summary>
    /// Verifies that the PauseDownload method does not throw any exceptions.
    /// </summary>
    [Fact]
    public void PauseDownload_ShouldNotThrowException()
    {
        // Arrange: Mock the IHttpDownloader and create a DownloadManager instance.
        var httpDownloader = Substitute.For<IHttpDownloader>();
        var downloadManager = new DownloadManager(httpDownloader);

        // Act & Assert: Call PauseDownload and ensure no exception is thrown.
        var exception = Record.Exception(() => downloadManager.PauseDownload("test-id"));
        Assert.Null(exception);
    }

    /// <summary>
    /// Verifies that the ResumeDownload method does not throw any exceptions.
    /// </summary>
    [Fact]
    public void ResumeDownload_ShouldNotThrowException()
    {
        // Arrange: Mock the IHttpDownloader and create a DownloadManager instance.
        var httpDownloader = Substitute.For<IHttpDownloader>();
        var downloadManager = new DownloadManager(httpDownloader);

        // Act & Assert: Call ResumeDownload and ensure no exception is thrown.
        var exception = Record.Exception(() => downloadManager.ResumeDownload("test-id"));
        Assert.Null(exception);
    }
}
