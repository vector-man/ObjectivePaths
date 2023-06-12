using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectivePaths.IO
{
    public interface IAsyncFile : IAsyncPath, IFile, INotifyPropertyChanged
    {
        Task CopyToAsync(IAsyncFile destination, bool overwrite, CancellationToken token);
        Task MoveToAsync(IAsyncFile destination, bool overwrite, CancellationToken token);
        Task<Stream> OpenAsync(FileMode mode, FileAccess access, FileShare sharing, CancellationToken cancellationToken);
    }
}