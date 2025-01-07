using System;
using System.Windows;
using AnyDownloader.Core.Interfaces;
using AnyDownloader.Presentation.Views;

namespace AnyDownloader.Presentation
{
    /// <summary>
    /// Main application window dynamically loading DownloaderPage.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDownloadManager _downloadManager;
        private readonly ILogger _logger;

        public MainWindow(IDownloadManager downloadManager, ILogger logger)
        {
            InitializeComponent();

            _downloadManager = downloadManager ?? throw new ArgumentNullException(nameof(downloadManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("[MainWindow] Initializing MainWindow.");

            // Dynamically load DownloaderPage
            Content = new DownloaderPage(_downloadManager, _logger);

            _logger.LogInformation("[MainWindow] DownloaderPage loaded successfully.");
        }
    }
}
