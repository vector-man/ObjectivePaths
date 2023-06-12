using System;

namespace ObjectivePaths.IO
{
    public interface IFileSystemOperations : IDisposable
    {
        /// <summary>
        /// Copies the specified <paramref name="sourceFileName"/> to <paramref name="destFileName"/>.
        /// </summary>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName">The name of the destination file.</param>
        /// <param name="overwrite">true if the destination file can be overwritten, otherwise false.</param>
        void CopyFile(string sourceFileName, string destFileName, bool overwrite);

        /// <summary>
        /// Copies the specified <paramref name="sourceDirectory"/> to <paramref name="destDirectory"/>.
        /// </summary>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName">The name of the destination file.</param>
        /// <param name="overwrite">true if the destination file can be overwritten, otherwise false.</param>
        void CopyDirectory(string sourceDirectory, string destDirectory, bool overwrite);

        /// <summary>
        /// Moves the specified <paramref name="sourceDirectory"/> to <paramref name="destDirectory"/>.
        /// </summary>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName">The name of the destination file.</param>
        /// <param name="overwrite">true if the destination file can be overwritten, otherwise false.</param>
        void MoveDirectory(string sourceDirectory, string destDirectory, bool overwrite);

        /// <summary>
        /// Creates all directories in the specified path.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Deletes the specified file. An exception is not thrown if the file does not exist.
        /// </summary>
        /// <param name="path">The file to be deleted.</param>
        void DeleteFile(string fileName);

        /// <summary>
        /// Deletes the specified directory and all its contents. An exception is not thrown if the directory does not exist.
        /// </summary>
        /// <param name="path">The directory to be deleted.</param>
        void DeleteDirectory(string path);

        /// <summary>
        /// Moves the specified file to a new location.
        /// </summary>
        /// <param name="srcFileName">The name of the file to move.</param>
        /// <param name="destFileName">The new path for the file.</param>
        void MoveFile(string srcFileName, string destFileName);
    }
}