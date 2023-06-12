using System.Collections.Generic;

namespace ObjectivePaths.IO
{
    public interface IDirectory : IPath
    {
        void CopyTo(IDirectory destination, bool overwrite);

        void Create();

        void Create(bool createParent);

        void Delete(bool recursive);

        IEnumerable<IAsyncPath> GetChildren();

        IEnumerable<IAsyncDirectory> GetDirectories(bool recursive);

        IEnumerable<IAsyncDirectory> GetDirectories();

        IEnumerable<IAsyncFile> GetFiles(bool recursive);

        IEnumerable<IAsyncFile> GetFiles();

        void MoveTo(IDirectory destination, bool overwrite);

        IAsyncPath GetPath(string name);

        IAsyncPath GetPath(string name, PathType pathType);

        IAsyncDirectory GetDirectory(string name);

        IAsyncFile GetFile(string name);
    }
}