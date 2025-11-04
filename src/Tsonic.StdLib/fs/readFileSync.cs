using System.Text;

namespace Tsonic.StdLib;

public static partial class fs
{
    /// <summary>
    /// Synchronously reads the entire contents of a file.
    /// </summary>
    /// <param name="path">Filename or file path.</param>
    /// <param name="encoding">Character encoding (e.g., "utf-8"). If null, returns Buffer.</param>
    /// <returns>The contents of the file as a string.</returns>
    public static string readFileSync(string path, string? encoding = "utf-8")
    {
        if (encoding == null || encoding.ToLowerInvariant() == "buffer")
        {
            throw new NotSupportedException("Buffer return type not yet implemented. Use string encoding.");
        }

        var enc = ParseEncoding(encoding);
        return File.ReadAllText(path, enc);
    }
}
