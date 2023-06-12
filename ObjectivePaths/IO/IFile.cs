using System.IO;

namespace ObjectivePaths.IO
{
    public interface IFile : IPath
    {
        IAsyncDirectory Directory { get; }
        bool? Hidden { get; set; }

        long Length { get; }

        bool? ReadOnly { get; set; }

        void CopyTo(IFile destination, bool overwrite);

        void MoveTo(IFile destination, bool overwrite);

        Stream Open(FileMode mode, FileAccess access, FileShare sharing);

        void Close();
    }
}