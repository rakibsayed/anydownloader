using System.IO;

namespace AnyDownloader.Core.Interfaces
{
    /// <summary>
    /// Provides an abstraction for file system operations to enable better testability and decoupling.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Writes the specified content to a file at the given path.
        /// </summary>
        /// <param name="path">The file path where the content will be written.</param>
        /// <param name="content">The content to write to the file.</param>
        void WriteFile(string path, string content);

        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file to delete.</param>
        void DeleteFile(string path);

        /// <summary>
        /// Creates a file stream for the specified file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="mode">The mode to open the file in.</param>
        /// <param name="access">The access permissions for the file stream.</param>
        /// <param name="share">The sharing permissions for the file stream.</param>
        /// <returns>A <see cref="FileStream"/> instance for the file.</returns>
        FileStream CreateFileStream(string path, FileMode mode, FileAccess access, FileShare share);

        /// <summary>
        /// Renames or moves a file from the source path to the destination path.
        /// </summary>
        /// <param name="sourcePath">The path to the source file.</param>
        /// <param name="destinationPath">The new path for the file.</param>
        void RenameFile(string sourcePath, string destinationPath);

        /// <summary>
        /// Creates a directory at the specified path if it does not already exist.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        void CreateDirectory(string path);
    }
}
