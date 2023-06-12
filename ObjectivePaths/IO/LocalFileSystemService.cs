using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.Linq;
using ObjectivePaths.Utils;

namespace ObjectivePaths.IO
{
    public class LocalFileSystemService : IFileSystemService
    {
        private readonly SemaphoreSlim _semaphore;

        public IFileSystem FileSystem { get; }

        public LocalFileSystemService(IFileSystem fileSystem)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _semaphore = new SemaphoreSlim(1);
        }

        public IAsyncDirectory GetTemporaryDirectoryRoot()
        {
            var tempPath = FileSystem.Path.GetTempPath();
            return GetDirectory(tempPath);
        }

        public async Task<Stream> OpenFileAsync(IAsyncFile file, FileMode mode, FileAccess access, FileShare sharing, CancellationToken cancellationToken)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                return FileSystem.File.Open(file.AbsolutePath, mode, access, sharing);
            }
            finally
            {
                {
                    _semaphore.Release();
                }
            }
        }


        public string GetNativePath(string path)
        {
            var nativePath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (nativePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
                nativePath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                return nativePath.Remove(nativePath.Length - 1);
            }

            return nativePath;
        }
        public IAsyncPath GetPath(string path)
        {
            char end = path.Trim()[path.Length - 1];

            if (end == Path.DirectorySeparatorChar || end == Path.AltDirectorySeparatorChar)
            {
                return GetDirectory(path);
            }

            return GetFile(path);
        }

        public IAsyncPath GetPath(string path, PathType pathType)
        {
            {
                if (pathType == PathType.Directory)
                {
                    return GetDirectory(path);
                }
                else
                {
                    return GetFile(path);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public IAsyncDirectory GetDirectory(string path)
        {
            var nativePath = GetNativePath(path);
            return new LocalDirectory(this, FileSystem.DirectoryInfo.New(nativePath + Path.DirectorySeparatorChar));
        }

        public IEnumerable<IAsyncDirectory> GetDirectories(string path)
        {
            var nativePath = GetNativePath(path);
            var directories = FileSystem.Directory.GetDirectories(nativePath)
                .Select(x => new LocalDirectory(this, FileSystem.DirectoryInfo.New(x + Path.DirectorySeparatorChar)));

            return directories;
        }


        public IAsyncFile GetFile(string path)
        {
            var nativeFilePath = GetNativePath(path);

            IFileInfo fileInfo = FileSystem.FileInfo.New(PathUtils.LongPathPrefix + path);

            return new LocalAsyncFile(this, fileInfo);
        }

    }
}