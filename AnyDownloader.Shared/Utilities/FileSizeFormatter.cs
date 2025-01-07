using System;

namespace AnyDownloader.Shared.Utilities
{
    /// <summary>
    /// Provides utility methods for converting file sizes to human-readable formats.
    /// </summary>
    public static class FileSizeFormatter
    {
        /// <summary>
        /// Converts a file size in bytes to a human-readable string using appropriate units (B, KB, MB, GB, etc.).
        /// </summary>
        /// <param name="bytes">The file size in bytes. Should be a non-negative value.</param>
        /// <returns>A formatted string representing the file size in the most appropriate unit.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the byte value is negative.</exception>
        public static string FormatSize(long bytes)
        {
            if (bytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "File size cannot be negative.");
            }

            // If the size is less than 1 KB, return in bytes
            if (bytes < 1024)
            {
                return $"{bytes} B";
            }

            // Define the units for file size
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

            // Determine the most appropriate unit for the file size
            int unitIndex = 0;
            double size = bytes;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            // Return the size formatted to 2 decimal places
            return $"{size:F2} {units[unitIndex]}";
        }
    }
}
