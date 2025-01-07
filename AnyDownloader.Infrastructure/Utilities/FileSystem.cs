using System;
using System.IO;
using AnyDownloader.Core.Interfaces;

namespace AnyDownloader.Infrastructure.Utilities
{
    /// <summary>
    /// Provides a concrete implementation of <see cref="IFileSystem"/> for performing file system operations.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        /// <inheritdoc />
        public void WriteFile(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "File path cannot be null or empty.");
            }

            File.WriteAllText(path, content);
        }

        /// <inheritdoc />
        public bool FileExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "File path cannot be null or empty.");
            }

            return File.Exists(path);
        }

        /// <inheritdoc />
        public void DeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "File path cannot be null or empty.");
            }

            if (FileExists(path))
            {
                File.Delete(path);
            }
            else
            {
                throw new FileNotFoundException("File not found for deletion.", path);
            }
        }

        /// <inheritdoc />
        public FileStream CreateFileStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "File path cannot be null or empty.");
            }

            return new FileStream(path, mode, access, share);
        }

        /// <inheritdoc />
        public void RenameFile(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new ArgumentNullException(nameof(sourcePath), "Source path cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new ArgumentNullException(nameof(destinationPath), "Destination path cannot be null or empty.");
            }

            if (File.Exists(sourcePath))
            {
                File.Move(sourcePath, destinationPath, true);
            }
            else
            {
                throw new FileNotFoundException("Source file not found for renaming.", sourcePath);
            }
        }

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "Directory path cannot be null or empty.");
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
