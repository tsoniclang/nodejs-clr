namespace nodejs;

/// <summary>
/// Singleton class representing the path module for posix/win32 properties.
/// </summary>
public class PathModule
{
    private static readonly Lazy<PathModule> _instance = new(() => new PathModule());

    /// <summary>Gets the singleton instance of PathModule.</summary>
    public static PathModule Instance => _instance.Value;

    private PathModule() { }

    /// <summary>Platform-specific path segment separator.</summary>
    public string sep => path.sep;

    /// <summary>Platform-specific path delimiter for environment variables.</summary>
    public string delimiter => path.delimiter;

    /// <summary>Returns the last portion of a path.</summary>
    public string basename(string path, string? suffix = null) => nodejs.path.basename(path, suffix);

    /// <summary>Returns the directory name of a path.</summary>
    public string dirname(string path) => nodejs.path.dirname(path);

    /// <summary>Returns the extension of the path.</summary>
    public string extname(string path) => nodejs.path.extname(path);

    /// <summary>Joins all path segments together.</summary>
    public string join(params string[] paths) => nodejs.path.join(paths);

    /// <summary>Normalizes the given path.</summary>
    public string normalize(string path) => nodejs.path.normalize(path);

    /// <summary>Resolves a sequence of paths to an absolute path.</summary>
    public string resolve(params string[] paths) => nodejs.path.resolve(paths);

    /// <summary>Determines if path is an absolute path.</summary>
    public bool isAbsolute(string path) => nodejs.path.isAbsolute(path);

    /// <summary>Returns the relative path from 'from' to 'to'.</summary>
    public string relative(string from, string to) => nodejs.path.relative(from, to);

    /// <summary>Returns an object representing significant elements of the path.</summary>
    public ParsedPath parse(string path) => nodejs.path.parse(path);

    /// <summary>Returns a path string from an object.</summary>
    public string format(ParsedPath pathObject) => nodejs.path.format(pathObject);

    /// <summary>Determines if path matches the glob pattern.</summary>
    public bool matchesGlob(string path, string pattern) => nodejs.path.matchesGlob(path, pattern);

    /// <summary>Returns an equivalent namespace-prefixed path.</summary>
    public string toNamespacedPath(string path) => nodejs.path.toNamespacedPath(path);

    /// <summary>POSIX-specific path operations.</summary>
    public PathModule posix => Instance;

    /// <summary>Windows-specific path operations.</summary>
    public PathModule win32 => Instance;
}
