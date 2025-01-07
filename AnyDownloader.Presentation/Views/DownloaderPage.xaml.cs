using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using AnyDownloader.Core.Interfaces;

namespace AnyDownloader.Presentation.Views
{
    /// <summary>
    /// Interaction logic for DownloaderPage.xaml
    /// </summary>
    public partial class DownloaderPage : UserControl
    {
        private readonly IDownloadManager _downloadManager;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloaderPage"/> class.
        /// </summary>
        /// <param name="downloadManager">The download manager used for handling download operations.</param>
        /// <param name="logger">The logger used for logging events and errors.</param>
        public DownloaderPage(IDownloadManager downloadManager, ILogger logger)
        {
            InitializeComponent();
            _downloadManager = downloadManager ?? throw new ArgumentNullException(nameof(downloadManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the KeyDown event for the Download URL TextBox.
        /// Starts the download process when the Enter key is pressed.
        /// </summary>
        private void DownloadUrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartDownloadProcess();
            }
        }

        /// <summary>
        /// Handles the Click event for the Start Download Button.
        /// </summary>
        private void StartDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            StartDownloadProcess();
        }

        /// <summary>
        /// Initiates the download process.
        /// </summary>
        private async void StartDownloadProcess()
        {
            string downloadUrl = DownloadUrlTextBox.Text.Trim();

            // Validate the URL
            if (!ValidateUrl(downloadUrl))
            {
                UpdateStatusMessage("Invalid URL format! Please enter a valid URL.", false);
                return;
            }

            // Show "Validating URL" message and toggle spinner
            UpdateStatusMessage("Validating URL...", true);
            ToggleSpinnerVisibility(true);

            try
            {
                // Perform the download operation
                await PerformDownloadAsync(downloadUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StartDownloadProcess] Error during download: {ex.Message}", ex);
                UpdateStatusMessage($"Error: {ex.Message}", false);
            }
            finally
            {
                // Hide spinner and show the button
                ToggleSpinnerVisibility(false);
            }
        }

        /// <summary>
        /// Performs the download operation using the Download Manager.
        /// </summary>
        /// <param name="downloadUrl">The URL to download the file from.</param>
        private async Task PerformDownloadAsync(string downloadUrl)
        {
            string? filePath = null;

            await _downloadManager.StartDownloadAsync(
                downloadUrl,
                null,
                progress =>
                {
                    filePath = progress.FilePath;
                    Dispatcher.Invoke(() =>
                    {
                        UpdateStatusMessage($"Downloading {progress.FileName}... {Math.Floor(progress.Percentage)}%", true);
                    });
                });

            if (!string.IsNullOrEmpty(filePath))
            {
                UpdateStatusMessage($"Download completed! File saved to: {filePath}", true);
            }
        }

        /// <summary>
        /// Toggles visibility between the spinner and the Start Download button.
        /// </summary>
        /// <param name="showSpinner">True to show the spinner, false to show the button.</param>
        private void ToggleSpinnerVisibility(bool showSpinner)
        {
            if (showSpinner)
            {
                StartDownloadButton.Visibility = Visibility.Collapsed;
                SpinnerCanvas.Visibility = Visibility.Visible;

                if (SpinnerCanvas.Resources["SpinnerAnimation"] is Storyboard spinnerAnimation)
                {
                    spinnerAnimation.Begin(SpinnerCanvas, true);
                }
                else
                {
                    _logger.LogWarning("[ToggleSpinnerVisibility] SpinnerAnimation resource not found.");
                }
            }
            else
            {
                StartDownloadButton.Visibility = Visibility.Visible;
                SpinnerCanvas.Visibility = Visibility.Collapsed;

                if (SpinnerCanvas.Resources["SpinnerAnimation"] is Storyboard spinnerAnimation)
                {
                    spinnerAnimation.Stop(SpinnerCanvas);
                }
            }
        }

        /// <summary>
        /// Updates the status message below the TextBox.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="isSuccess">True for success messages, false for error messages.</param>
        private void UpdateStatusMessage(string message, bool isSuccess)
        {
            StatusMessageTextBlock.Text = message;
            StatusMessageTextBlock.Foreground = isSuccess
                ? System.Windows.Media.Brushes.LightGreen
                : System.Windows.Media.Brushes.OrangeRed;
            StatusMessageTextBlock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Validates the given URL format.
        /// </summary>
        /// <param name="url">The URL to validate.</param>
        /// <returns>True if the URL is valid, otherwise false.</returns>
        private bool ValidateUrl(string url)
        {
            return !string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        /// <summary>
        /// Event handler for the TextChanged event of the DownloadUrlTextBox.
        /// Hides the status message when the user starts typing in the TextBox.
        /// </summary>
        private void DownloadUrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StatusMessageTextBlock.Visibility == Visibility.Visible)
            {
                StatusMessageTextBlock.Visibility = Visibility.Collapsed;
                StatusMessageTextBlock.Text = string.Empty;
            }
        }
    }
}
