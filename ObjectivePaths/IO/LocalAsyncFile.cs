using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectivePaths.IO
{
    public class LocalAsyncFile : IAsyncFile
    {
        private readonly IFileInfo _fileInfo;
        private readonly IFileSystemService _fileSystemService;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private readonly Stream _stream = null;


        public LocalAsyncFile(IFileSystemService fileSystemService, IFileInfo fileInfo)
        {
            _fileSystemService = fileSystemService;
            _fileInfo = fileInfo;
        }

        public string AbsolutePath => GetNativePath();

        public bool Exists => _fileInfo.Exists;

        public IFileSystem FileSystem => _fileSystemService.FileSystem;

        public IAsyncDirectory Directory => _fileSystemService.GetDirectory(_fileInfo?.Directory?.FullName);

        public bool? Hidden
        {
            get => _fileInfo.Attributes.HasFlag(FileAttributes.Hidden);

            set
            {
                if (value.HasValue && value.Value)
                {
                    _fileInfo.Attributes |= FileAttributes.Hidden;
                }
                else
                {
                    _fileInfo.Attributes &= ~FileAttributes.Hidden;
                }
            }
        }

        public bool IsRoot => false;

        public long Length => !_fileInfo.Exists ? 0 : _fileInfo.Length;

        public string Name => _fileInfo.Name;

        public IAsyncDirectory ParentDirectory =>
            _fileSystemService.GetDirectory(_fileInfo.Directory.Parent.FullName);

        public PathType PathType => PathType.File;

        public bool? ReadOnly
        {
            get
            {
                if (Exists)
                {
                    return _fileInfo.IsReadOnly;
                }

                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _fileInfo.IsReadOnly = value.Value;
                }
            }
        }

        public void CopyTo(IPath destination, bool overwrite)
        {
            CopyTo((IFile)destination, overwrite);
        }

        public void CopyTo(IFile destination, bool overwrite)
        {

            var overwriteMode = overwrite ? FileMode.Create : FileMode.CreateNew;

            using (var src = _fileInfo.OpenRead())
            using (var dest = destination.Open(overwriteMode, FileAccess.Write, FileShare.None))
            {
                src.CopyTo(dest, 81920);
            }
        }

        public async Task CopyToAsync(IAsyncPath destination, bool overwrite, CancellationToken token)
        {

            await CopyToAsync((IAsyncFile)destination, overwrite, token);

        }

        public async Task CopyToAsync(IAsyncFile destination, bool overwrite, CancellationToken token)
        {
            try
            {
                await _semaphore.WaitAsync(token);
                var overwriteMode = overwrite ? FileMode.Create : FileMode.CreateNew;

                using (Stream src = _fileInfo.OpenRead())
                using (Stream dest = destination.Open(overwriteMode, FileAccess.Write, FileShare.None))
                {
                    await src.CopyToAsync(dest, 81920, token);
                }

            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Delete()
        {
            _fileInfo.Delete();
        }

        private string GetNativePath()
        {
            return _fileSystemService.GetNativePath(_fileInfo?.FullName);
        }

        public void MoveTo(IFile destination, bool overwrite)
        {
            CopyTo(destination, overwrite);
            Delete();
        }

        public async Task MoveToAsync(IAsyncFile destination, bool overwrite, CancellationToken token)
        {
            try
            {
                await _semaphore.WaitAsync(token);
                await CopyToAsync(destination, overwrite, token);
                await Task.Run(Delete, token);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Stream> OpenAsync(FileMode mode, FileAccess access, FileShare sharing,
            CancellationToken cancellationToken)
        {

            var stream = await _fileSystemService.OpenFileAsync(this, mode, access, sharing, cancellationToken);
            return stream;
        }

        public Stream Open(FileMode mode, FileAccess access, FileShare sharing)
        {
            try
            {
                _semaphore.Wait();

                var stream = _fileInfo.Open(mode, access, sharing);

                return stream;
            }
            finally
            {
                {
                    _semaphore.Release();
                }


            }
        }

        public void Close()
        {
            _stream?.Close();
            _stream?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}