using System.Threading.Tasks;

namespace Tsonic.Node;

public static partial class fs
{
    /// <summary>
    /// Asynchronously retrieves statistics for the file/directory at the given path.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    /// <returns>A promise that resolves to a Stats object.</returns>
    public static Task<Stats> stat(string path)
    {
        return Task.Run(() =>
        {
            var fileInfo = new FileInfo(path);
            var dirInfo = new DirectoryInfo(path);

            var isFile = fileInfo.Exists;
            var isDir = dirInfo.Exists;

            if (!isFile && !isDir)
            {
                throw new FileNotFoundException($"No such file or directory: {path}");
            }

            if (isFile)
            {
                return new Stats
                {
                    size = fileInfo.Length,
                    mode = 0,
                    atime = fileInfo.LastAccessTime,
                    mtime = fileInfo.LastWriteTime,
                    ctime = fileInfo.CreationTime,
                    birthtime = fileInfo.CreationTime,
                    isFile = true,
                    isDirectory = false
                };
            }
            else
            {
                return new Stats
                {
                    size = 0,
                    mode = 0,
                    atime = dirInfo.LastAccessTime,
                    mtime = dirInfo.LastWriteTime,
                    ctime = dirInfo.CreationTime,
                    birthtime = dirInfo.CreationTime,
                    isFile = false,
                    isDirectory = true
                };
            }
        });
    }
}
