using System.Threading;
using System.Threading.Tasks;

namespace ObjectivePaths.IO
{
    public interface IAsyncDirectory : IAsyncPath, IDirectory
    {
        Task CopyToAsync(IAsyncDirectory destination, bool overwrite, CancellationToken token);

        Task MoveToAsync(IAsyncDirectory destination, bool overwrite, CancellationToken token);
    }
}