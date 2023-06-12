using System.IO.Abstractions;

namespace ObjectivePaths.IO
{
    public interface IPath
    {
        string AbsolutePath { get; }

        bool Exists { get; }

        IFileSystem FileSystem { get; }

        bool IsRoot { get; }

        string Name { get; }

        IAsyncDirectory ParentDirectory { get; }

        PathType PathType { get; }

        void Delete();
        void CopyTo(IPath destination, bool overwrite);
    }
}