using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using ObjectivePaths.Utils;

namespace ObjectivePaths.IO
{
    public class LocalDirectory : IAsyncDirectory
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IDirectoryInfo _directoryInfo;

        public LocalDirectory(IFileSystemService fileSystemService, IDirectoryInfo directory)
        {
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _directoryInfo = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        public string AbsolutePath => GetNativePath();

        public bool Exists
        {
            get
            {
                _directoryInfo.Refresh();
                return _directoryInfo.Exists;
            }
        }

        public IFileSystem FileSystem => _fileSystemService.FileSystem;

        public bool IsRoot => _directoryInfo.Parent == null;

        public string Name => _directoryInfo.Name;

        public IAsyncDirectory ParentDirectory
        {
            get
            {
                if (IsRoot) throw new InvalidOperationException("Directory is root.");

                return _fileSystemService.GetDirectory(_directoryInfo?.Parent?.FullName);
            }
        }

        public PathType PathType => PathType.Directory;

        public void CopyTo(IPath destination, bool overwrite)
        {
            if (destination.PathType != PathType.Directory)
            {
                throw new ArgumentException("Destination is not a directory.");
            }

            CopyTo((IDirectory)destination, overwrite);
        }

        public void CopyTo(IDirectory destination, bool overwrite)
        {
            // Create all absolute directory paths.
            foreach (var dir in GetDirectories())
            {
                string relativePath = this.GetRelativePathTo(dir.AbsolutePath);

                IDirectory newDirectory = destination.GetDirectory(relativePath);
                newDirectory.Create();
            }

            foreach (var file in GetFiles(true))
            {
                var relativePath = this.GetRelativePathTo(file.AbsolutePath);

                IFile newFile = destination.GetFile(relativePath);

                file.CopyTo(newFile, overwrite);
            }
        }

        public async Task CopyToAsync(IAsyncPath destination, bool overwrite, CancellationToken token)
        {
            if (destination.PathType != PathType.Directory)
            {
                throw new ArgumentException("Destination is not a directory.");
            }

            await CopyToAsync((IAsyncDirectory)destination, overwrite, token);
        }

        public async Task CopyToAsync(IAsyncDirectory destination, bool overwrite, CancellationToken token)
        {
            // Create all absolute directory paths.
            foreach (var dir in GetDirectories())
            {
                token.ThrowIfCancellationRequested();

                var relativePath = this.GetRelativePathTo(dir.AbsolutePath);

                var newDirectory = destination.GetDirectory(relativePath);
                newDirectory.Create();
            }

            foreach (var file in GetFiles(true))
            {
                if (token.IsCancellationRequested) return;

                var relativePath = this.GetRelativePathTo(file.AbsolutePath);

                var newFile = destination.GetFile(relativePath);

                await file.CopyToAsync(newFile, overwrite, token);
            }
        }

        public void Create()
        {
            Create(false);
        }

        public void Create(bool createParent)
        {
            if (createParent)
            {
                Directory.CreateDirectory(_directoryInfo.FullName);
            }
            else
            {
                _directoryInfo.Create();
            }
        }

        public void Delete()
        {
            Delete(false);
        }

        public void Delete(bool recursive)
        {
            _directoryInfo.Delete(recursive);
        }

        public IEnumerable<IAsyncPath> GetChildren()
        {
            foreach (var directory in GetDirectories())
            {
                yield return directory;
            }

            foreach (var file in GetFiles())
            {
                yield return file;
            }
        }

        public IEnumerable<IAsyncDirectory> GetDirectories(bool recursive)
        {
            var option = recursive ?
                                  SearchOption.AllDirectories :
                                  SearchOption.TopDirectoryOnly;

            foreach (var directory in
                     _fileSystemService.GetDirectories(PathUtils.LongPathPrefix + AbsolutePath))
            {
                var newDirectoryInfo = _fileSystemService.FileSystem.DirectoryInfo.New(directory.AbsolutePath);

                yield return new LocalDirectory(_fileSystemService, newDirectoryInfo);
            }
        }

        public IEnumerable<IAsyncDirectory> GetDirectories()
        {
            return _fileSystemService.GetDirectories(this.AbsolutePath);
        }

        public IEnumerable<IAsyncFile> GetFiles(bool recursive)
        {
            var option = recursive ? SearchOption.AllDirectories :
                                     SearchOption.TopDirectoryOnly;

            foreach (var file in
                     _fileSystemService.FileSystem.Directory.EnumerateFiles(_directoryInfo.FullName, "*", option))
            {
                var newFileInfo = _fileSystemService.FileSystem.FileInfo.New(file);

                yield return new LocalAsyncFile(_fileSystemService, newFileInfo);
            }
        }

        public IEnumerable<IAsyncFile> GetFiles()
        {
            return GetFiles(false);
        }


        public void MoveTo(IDirectory destination, bool overwrite)
        {
            CopyTo(destination, overwrite);
            Delete(true);
        }

        public async Task MoveToAsync(IAsyncDirectory destination, bool overwrite, CancellationToken token)
        {
            await CopyToAsync(destination, overwrite, token);
            await Task.Run(() => Delete(true), token);
        }

        public IAsyncPath GetPath(string name)
        {
            return _fileSystemService.GetPath(AbsolutePath + name);
        }

        public IAsyncPath GetPath(string name, PathType pathType)
        {
            return _fileSystemService.GetPath(AbsolutePath + name, pathType);
        }

        public IAsyncDirectory GetDirectory(string name)
        {
            return _fileSystemService.GetDirectory(AbsolutePath + name);
        }

        public IAsyncFile GetFile(string name)
        {
            return _fileSystemService.GetFile(AbsolutePath + name);
        }

        private string GetNativePath()
        {
            return _fileSystemService.GetNativePath(_directoryInfo?.FullName) + Path.DirectorySeparatorChar;
        }
    }
}