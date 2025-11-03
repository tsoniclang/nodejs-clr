using System.Text;
using System.Threading.Tasks;

namespace Tsonic.NodeApi;

public static partial class fs
{
    /// <summary>
    /// Asynchronously reads the entire contents of a file.
    /// </summary>
    /// <param name="path">Filename or file path.</param>
    /// <param name="encoding">Character encoding (e.g., "utf-8"). If null, returns Buffer.</param>
    /// <returns>A promise that resolves to the contents of the file as a string.</returns>
    public static async Task<string> readFile(string path, string? encoding = "utf-8")
    {
        if (encoding == null || encoding.ToLowerInvariant() == "buffer")
        {
            throw new NotSupportedException("Buffer return type not yet implemented. Use string encoding.");
        }

        var enc = ParseEncoding(encoding);
        return await File.ReadAllTextAsync(path, enc);
    }
}
