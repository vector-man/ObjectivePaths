using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ObjectivePaths.Utils;

namespace ObjectivePaths.IO
{
    public static class Extensions
    {
        public static string GetRelativePathTo(this IAsyncPath path, string absolutePath)
        {
            string fromPath = path.AbsolutePath;

            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrEmpty(absolutePath))
            {
                throw new ArgumentNullException(nameof(absolutePath));
            }

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(absolutePath);

            if (fromUri.Scheme != toUri.Scheme)
            {
                return absolutePath;
            }

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());


            return relativePath.TrimStart('.').Replace('/', Path.DirectorySeparatorChar);
        }

        public static async Task DeleteAsync(this IDirectory path, CancellationToken token)
        {
            await Task.Run(() => ((IAsyncDirectory)path).Delete(true), token);
        }


        /// <summary>
        /// Creates a unique, temporary directory and returns it as an IObjectiveDirectory object.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <returns></returns>
        public static IAsyncDirectory CreateTemporaryDirectory(this IFileSystemService fileSystemService)
        {
            var tempDir = fileSystemService.GetTemporaryDirectoryRoot().GetRandomDirectory();
            tempDir.Create();
            return tempDir;
        }

        public static IAsyncFile GetTemporaryFile(this IFileSystemService fileSystemService, string extension = "")
        {
            var tempPath = fileSystemService.GetTemporaryDirectoryRoot();
            return tempPath.GetFile(Path.GetRandomFileName() + extension);
        }

        public static bool NameExists(this IAsyncPath path)
        {
            var exists = false;

            if (!path.IsRoot)
            {
                exists = (path as IAsyncFile)?.Directory?.GetFile(path.Name)?.Exists == true;
                exists |= path.ParentDirectory.GetDirectory(path.Name).Exists;
            }

            return exists;
        }
        
        public static string GetSafeName(this IAsyncPath path)
        {
            if (path.IsRoot)
            {
                var root = PathUtils.GetSafePathName(path.AbsolutePath, string.Empty);
                return string.IsNullOrWhiteSpace(root) ? "Root" : root;
            }

            return PathUtils.GetSafePathName(Path.GetFileName(path.Name));
        }

        private static IEnumerable<string> GetDirectoryRelativePaths(IAsyncDirectory dir)
        {
            return dir.GetDirectories().Where((d) => !d.GetDirectories().Any()).AsParallel()
                .Select((d) => dir.GetRelativePathTo(d.AbsolutePath)).OrderBy((p) => p);
        }

        public static IAsyncDirectory GetRandomDirectory(this IAsyncDirectory directory)
        {
            var randomDirectoryName = directory.FileSystem.Path.GetRandomFileName();
            return directory.GetDirectory(randomDirectoryName);
        }

    }
}
