namespace Tsonic.StdLib;

/// <summary>
/// Options for creating a dgram socket.
/// </summary>
public class SocketOptions
{
    /// <summary>
    /// The type of socket - 'udp4' or 'udp6'.
    /// </summary>
    public string type { get; set; } = "udp4";

    /// <summary>
    /// If true, the socket will reuse the address.
    /// </summary>
    public bool reuseAddr { get; set; } = false;

    /// <summary>
    /// If true, the socket will reuse the port (SO_REUSEPORT).
    /// </summary>
    public bool reusePort { get; set; } = false;

    /// <summary>
    /// For IPv6 sockets, if true the socket will only receive IPv6 traffic.
    /// </summary>
    public bool ipv6Only { get; set; } = false;

    /// <summary>
    /// Sets the SO_RCVBUF socket receive buffer size in bytes.
    /// </summary>
    public int? recvBufferSize { get; set; }

    /// <summary>
    /// Sets the SO_SNDBUF socket send buffer size in bytes.
    /// </summary>
    public int? sendBufferSize { get; set; }
}
