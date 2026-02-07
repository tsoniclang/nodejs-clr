using System;
using System.Threading.Tasks;

namespace nodejs;

/// <summary>
/// Promise-based readline wrappers.
/// </summary>
public class ReadlinePromises
{
    /// <summary>
    /// Creates a readline interface from options.
    /// </summary>
    public Interface createInterface(InterfaceOptions options)
    {
        return readline.createInterface(options);
    }

    /// <summary>
    /// Creates a readline interface from input/output streams.
    /// </summary>
    public Interface createInterface(Readable input, Writable? output = null)
    {
        return readline.createInterface(input, output);
    }

    /// <summary>
    /// Asks a question and resolves with the response.
    /// </summary>
    public Task<string> question(Interface rl, string query)
    {
        if (rl == null)
            throw new ArgumentNullException(nameof(rl));

        return rl.questionAsync(query);
    }
}
