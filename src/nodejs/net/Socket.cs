using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace nodejs;

#pragma warning disable CS8981 // Lowercase type names
#pragma warning disable IDE1006 // Naming rule violation

/// <summary>
/// This class is an abstraction of a TCP socket or a streaming IPC endpoint.
/// It is also an EventEmitter.
/// </summary>
public class Socket : Stream
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private bool _connecting = false;
    private bool _destroyed = false;
    private int _timeout = 0;
    private bool _allowHalfOpen = false;

    /// <summary>
    /// The amount of received bytes.
    /// </summary>
    public long bytesRead { get; private set; }

    /// <summary>
    /// The amount of bytes sent.
    /// </summary>
    public long bytesWritten { get; private set; }

    /// <summary>
    /// Whether the connection is active.
    /// </summary>
    public bool connecting => _connecting;

    /// <summary>
    /// Whether the socket has been destroyed.
    /// </summary>
    public bool destroyed => _destroyed;

    /// <summary>
    /// The string representation of the local IP address.
    /// </summary>
    public string? localAddress { get; private set; }

    /// <summary>
    /// The numeric representation of the local port.
    /// </summary>
    public int? localPort { get; private set; }

    /// <summary>
    /// The string representation of the local IP family.
    /// </summary>
    public string? localFamily { get; private set; }

    /// <summary>
    /// The string representation of the remote IP address.
    /// </summary>
    public string? remoteAddress { get; private set; }

    /// <summary>
    /// The numeric representation of the remote port.
    /// </summary>
    public int? remotePort { get; private set; }

    /// <summary>
    /// The string representation of the remote IP family.
    /// </summary>
    public string? remoteFamily { get; private set; }

    /// <summary>
    /// This property represents the state of the connection as a string.
    /// </summary>
    public string readyState
    {
        get
        {
            if (_destroyed) return "closed";
            if (_connecting) return "opening";
            if (_client?.Connected == true) return "open";
            return "closed";
        }
    }

    /// <summary>
    /// Creates a new Socket instance.
    /// </summary>
    public Socket() : this((SocketConstructorOpts?)null)
    {
    }

    /// <summary>
    /// Creates a new Socket instance with options.
    /// </summary>
    /// <param name="options">Socket constructor options</param>
    public Socket(SocketConstructorOpts? options) : base()
    {
        _allowHalfOpen = options?.allowHalfOpen ?? false;
        // Note: fd, readable, writable options not fully supported in .NET
    }

    internal Socket(TcpClient client) : base()
    {
        _client = client;
        _stream = client.GetStream();
        UpdateAddressInfo();
    }

    /// <summary>
    /// Initiate a connection on a given socket.
    /// </summary>
    /// <param name="port">Port to connect to</param>
    /// <param name="host">Host to connect to</param>
    /// <param name="connectionListener">Connection listener callback</param>
    /// <returns>The socket itself</returns>
    public Socket connect(int port, string? host = null, Action? connectionListener = null)
    {
        if (connectionListener != null)
        {
            once("connect", connectionListener);
        }

        _connecting = true;
        var hostname = host ?? "localhost";

        Task.Run(async () =>
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(hostname, port);
                _stream = _client.GetStream();
                _connecting = false;
                UpdateAddressInfo();
                emit("connect");
                emit("ready");
            }
            catch (Exception ex)
            {
                _connecting = false;
                emit("error", ex);
            }
        });

        return this;
    }

    /// <summary>
    /// Initiate a connection with options.
    /// </summary>
    /// <param name="options">Connection options</param>
    /// <param name="connectionListener">Connection listener callback</param>
    /// <returns>The socket itself</returns>
    public Socket connect(TcpSocketConnectOpts options, Action? connectionListener = null)
    {
        return connect(options.port, options.host, connectionListener);
    }

    /// <summary>
    /// Initiate a connection using a path (IPC).
    /// </summary>
    /// <param name="path">Path to connect to</param>
    /// <param name="connectionListener">Connection listener callback</param>
    /// <returns>The socket itself</returns>
    public Socket connect(string path, Action? connectionListener = null)
    {
        // IPC connections not fully supported in .NET cross-platform
        throw new NotSupportedException("IPC connections via path not supported");
    }

    /// <summary>
    /// Sends data on the socket.
    /// </summary>
    /// <param name="data">Data to write</param>
    /// <param name="callback">Callback when write completes</param>
    /// <returns>True if flushed to kernel buffer</returns>
    public bool write(byte[] data, Action<Exception?>? callback = null)
    {
        if (_stream == null || _destroyed)
        {
            callback?.Invoke(new InvalidOperationException("Socket not connected"));
            return false;
        }

        Task.Run(async () =>
        {
            try
            {
                await _stream.WriteAsync(data, 0, data.Length);
                bytesWritten += data.Length;
                callback?.Invoke(null);
                emit("drain");
            }
            catch (Exception ex)
            {
                callback?.Invoke(ex);
                emit("error", ex);
            }
        });

        return true;
    }

    /// <summary>
    /// Sends string data on the socket.
    /// </summary>
    /// <param name="data">String data to write</param>
    /// <param name="encoding">Encoding to use</param>
    /// <param name="callback">Callback when write completes</param>
    /// <returns>True if flushed to kernel buffer</returns>
    public bool write(string data, string? encoding = null, Action<Exception?>? callback = null)
    {
        var enc = encoding == null ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
        return write(enc.GetBytes(data), callback);
    }

    /// <summary>
    /// Half-closes the socket.
    /// </summary>
    /// <param name="callback">Callback when finished</param>
    /// <returns>The socket itself</returns>
    public Socket end(Action? callback = null)
    {
        if (_stream != null && !_destroyed)
        {
            _stream.Close();
            emit("end");
            callback?.Invoke();
        }
        return this;
    }

    /// <summary>
    /// Half-closes the socket after writing data.
    /// </summary>
    /// <param name="data">Data to write before closing</param>
    /// <param name="callback">Callback when finished</param>
    /// <returns>The socket itself</returns>
    public Socket end(byte[] data, Action? callback = null)
    {
        write(data, (err) =>
        {
            if (err == null)
            {
                end(callback);
            }
        });
        return this;
    }

    /// <summary>
    /// Half-closes the socket after writing string data.
    /// </summary>
    /// <param name="data">String data to write before closing</param>
    /// <param name="encoding">Encoding to use</param>
    /// <param name="callback">Callback when finished</param>
    /// <returns>The socket itself</returns>
    public Socket end(string data, string? encoding = null, Action? callback = null)
    {
        var enc = encoding == null ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
        return end(enc.GetBytes(data), callback);
    }

    /// <summary>
    /// Ensures that no more I/O activity happens on this socket.
    /// </summary>
    /// <param name="error">Optional error</param>
    /// <returns>The socket itself</returns>
    public new Socket destroy(Exception? error = null)
    {
        if (_destroyed) return this;

        _destroyed = true;
        _stream?.Close();
        _client?.Close();

        emit("close", error != null);
        if (error != null)
        {
            emit("error", error);
        }

        return this;
    }

    /// <summary>
    /// Destroys the socket after all data is written.
    /// </summary>
    public void destroySoon()
    {
        end(() => destroy());
    }

    /// <summary>
    /// Close the TCP connection by sending an RST packet.
    /// </summary>
    /// <returns>The socket itself</returns>
    public Socket resetAndDestroy()
    {
        _client?.Client?.Close(0);
        return destroy();
    }

    /// <summary>
    /// Set the encoding for the socket as a Readable Stream.
    /// </summary>
    /// <param name="encoding">Encoding name</param>
    /// <returns>The socket itself</returns>
    public Socket setEncoding(string? encoding = null)
    {
        // Encoding handling would be implemented with full stream support
        return this;
    }

    /// <summary>
    /// Pauses the reading of data.
    /// </summary>
    /// <returns>The socket itself</returns>
    public Socket pause()
    {
        // Pause would be implemented with full stream support
        return this;
    }

    /// <summary>
    /// Resumes reading after a call to socket.pause().
    /// </summary>
    /// <returns>The socket itself</returns>
    public Socket resume()
    {
        // Resume would be implemented with full stream support
        return this;
    }

    /// <summary>
    /// Sets the socket to timeout after timeout milliseconds of inactivity.
    /// </summary>
    /// <param name="timeout">Timeout in milliseconds</param>
    /// <param name="callback">Timeout callback</param>
    /// <returns>The socket itself</returns>
    public Socket setTimeout(int timeout, Action? callback = null)
    {
        _timeout = timeout;
        if (callback != null)
        {
            once("timeout", callback);
        }

        if (_stream != null)
        {
            _stream.ReadTimeout = timeout;
            _stream.WriteTimeout = timeout;
        }

        return this;
    }

    /// <summary>
    /// Enable/disable the use of Nagle's algorithm.
    /// </summary>
    /// <param name="noDelay">Disable Nagle's algorithm if true</param>
    /// <returns>The socket itself</returns>
    public Socket setNoDelay(bool noDelay = true)
    {
        if (_client?.Client != null)
        {
            _client.NoDelay = noDelay;
        }
        return this;
    }

    /// <summary>
    /// Enable/disable keep-alive functionality.
    /// </summary>
    /// <param name="enable">Enable keep-alive if true</param>
    /// <param name="initialDelay">Initial delay in milliseconds</param>
    /// <returns>The socket itself</returns>
    public Socket setKeepAlive(bool enable = false, int initialDelay = 0)
    {
        if (_client?.Client != null)
        {
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, enable);
        }
        return this;
    }

    /// <summary>
    /// Returns the bound address, the address family name and port of the socket.
    /// </summary>
    /// <returns>Address info or empty object</returns>
    public object address()
    {
        if (localAddress != null && localPort.HasValue && localFamily != null)
        {
            return new AddressInfo
            {
                address = localAddress,
                family = localFamily,
                port = localPort.Value
            };
        }
        return new { };
    }

    /// <summary>
    /// Calling unref() on a socket will allow the program to exit if this is the only active socket.
    /// </summary>
    /// <returns>The socket itself</returns>
    public Socket unref()
    {
        // Not applicable in .NET managed context
        return this;
    }

    /// <summary>
    /// Opposite of unref().
    /// </summary>
    /// <returns>The socket itself</returns>
    public Socket @ref()
    {
        // Not applicable in .NET managed context
        return this;
    }

    private void UpdateAddressInfo()
    {
        if (_client?.Client?.LocalEndPoint is IPEndPoint localEP)
        {
            localAddress = localEP.Address.ToString();
            localPort = localEP.Port;
            localFamily = localEP.AddressFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6";
        }

        if (_client?.Client?.RemoteEndPoint is IPEndPoint remoteEP)
        {
            remoteAddress = remoteEP.Address.ToString();
            remotePort = remoteEP.Port;
            remoteFamily = remoteEP.AddressFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6";
        }
    }

    /// <summary>
    /// Gets the underlying TcpClient (for TLS wrapping).
    /// </summary>
    internal TcpClient? GetTcpClient()
    {
        return _client;
    }
}

#pragma warning restore CS8981
#pragma warning restore IDE1006
