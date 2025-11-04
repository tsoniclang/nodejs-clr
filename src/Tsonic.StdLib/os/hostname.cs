using System.Net;

namespace Tsonic.StdLib;

public static partial class os
{
    /// <summary>
    /// Returns the host name of the operating system as a string.
    /// </summary>
    /// <returns>The hostname.</returns>
    public static string hostname()
    {
        return Dns.GetHostName();
    }
}
