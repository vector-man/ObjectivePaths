using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectivePaths.IO
{
    public interface IFileSystemService
    {
        IAsyncPath GetPath(string path);

        IAsyncPath GetPath(string path, PathType pathType);

        IAsyncDirectory GetDirectory(string path);

        IEnumerable<IAsyncDirectory> GetDirectories(string path);

        IAsyncFile GetFile(string path);

        IFileSystem FileSystem { get; }

        IAsyncDirectory GetTemporaryDirectoryRoot();
        Task<Stream> OpenFileAsync(IAsyncFile file, FileMode mode, FileAccess access, FileShare sharing, CancellationToken cancellationToken);

        string GetNativePath(string path);
    }
}