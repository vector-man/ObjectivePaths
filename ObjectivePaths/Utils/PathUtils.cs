using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using ObjectivePaths.IO;
namespace ObjectivePaths.Utils
{
    public class PathUtils
    {
        private static ILog logger = LogManager.GetLogger(nameof(PathUtils));
        private static List<string> _tempPaths = new List<string>();
        public const char DirectorySeparatorChar = '/';

        public static string LongPathPrefix => (OSUtils.IsUnix ?
            string.Empty :
            @"\\?\");

        public static string GetSafePathName(string name, string replace)
        {
            return string.Join("_", name.Split(Path.GetInvalidFileNameChars(),
                               StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }

        public static string GetSafePathName(string name)
        {
            return GetSafePathName(name, "_");
        }

        public static string GetTempPath()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _tempPaths.Add(tempFile);
            return tempFile;
        }

        public static bool PathExists(string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }


        public static string GetNativePath(string path)
        {
            return path.Replace(DirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public static IEnumerable<string> GetRelativePaths(IAsyncDirectory root, Predicate<IAsyncPath> filter)
        {
            IEnumerable<IAsyncPath> paths = root.GetChildren().Where((p) => filter(p));

            foreach (IAsyncFile path in paths)
            {
                yield return (root.GetRelativePathTo(path.AbsolutePath));
            }
        }
    }
}