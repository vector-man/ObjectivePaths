using System.Threading;
using System.Threading.Tasks;

namespace ObjectivePaths.IO
{
    public interface IAsyncPath : IPath
    {
        Task CopyToAsync(IAsyncPath destination, bool overwrite, CancellationToken token);
    }
}