using System;
using AnyDownloader.Shared.Utilities;

namespace AnyDownloader.Models
{
    /// <summary>
    /// Represents metadata for tracking download progress, including file size, speed, and estimated time remaining.
    /// </summary>
    public class DownloadProgress
    {
        /// <summary>
        /// Gets or sets the name of the file being downloaded.
        /// Default value is an empty string to prevent null reference issues.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full file path of the file being downloaded.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total size of the file being downloaded, in bytes.
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes downloaded so far.
        /// </summary>
        public long BytesDownloaded { get; set; }

        /// <summary>
        /// Gets or sets the download speed, measured in bytes per second.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Gets or sets the estimated time remaining for the download to complete.
        /// </summary>
        public TimeSpan TimeRemaining { get; set; }

        /// <summary>
        /// Gets the percentage of the download completed.
        /// If the total file size is unknown (set to 0), the percentage will be 0 to avoid division by zero.
        /// </summary>
        public double Percentage => TotalBytes > 0 ? (double)BytesDownloaded / TotalBytes * 100 : 0;

        /// <summary>
        /// Gets the human-readable representation of the total file size (e.g., MB, GB).
        /// </summary>
        public string TotalSizeFormatted => FileSizeFormatter.FormatSize(TotalBytes);

        /// <summary>
        /// Gets the human-readable representation of the downloaded size (e.g., MB, GB).
        /// </summary>
        public string DownloadedSizeFormatted => FileSizeFormatter.FormatSize(BytesDownloaded);

        /// <summary>
        /// Gets the human-readable representation of the download speed (e.g., MB/s).
        /// </summary>
        public string SpeedFormatted => $"{FileSizeFormatter.FormatSize((long)Speed)}/s";
    }
}
