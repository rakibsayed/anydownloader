using AnyDownloader.Core.Interfaces;
using System.Windows;

namespace AnyDownloader.Presentation
{
    public partial class MainWindow : Window
    {
        private readonly IDownloadManager _downloadManager;

        public MainWindow(IDownloadManager downloadManager)
        {
            InitializeComponent();
            _downloadManager = downloadManager;
            TestDownload();
        }

        private async void TestDownload()
        {
            string url = "https://images.unsplash.com/photo-1721332154191-ba5f1534266e?q=80&w=735&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDF8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"; // Replace with a valid URL
            string destinationPath = "C:\\Temp\\samplefile.jpg"; // Replace with your test path

            try
            {
                MessageBox.Show("Starting download...");
                await _downloadManager.StartDownloadAsync(url, destinationPath);
                MessageBox.Show("Download completed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Download failed: {ex.Message}");
            }
        }
    }
}
