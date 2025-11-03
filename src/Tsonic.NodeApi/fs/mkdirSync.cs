namespace Tsonic.NodeApi;

public static partial class fs
{
    /// <summary>
    /// Synchronously creates a directory.
    /// </summary>
    /// <param name="path">The directory path to create.</param>
    /// <param name="recursive">If true, creates parent directories as needed (default: false).</param>
    public static void mkdirSync(string path, bool recursive = false)
    {
        if (recursive)
        {
            Directory.CreateDirectory(path);
        }
        else
        {
            // Non-recursive: fail if parent doesn't exist
            var parent = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
            {
                throw new DirectoryNotFoundException($"Parent directory does not exist: {parent}");
            }
            Directory.CreateDirectory(path);
        }
    }
}
