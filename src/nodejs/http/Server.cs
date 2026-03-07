using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using nodejs;
using Tsonic.JSRuntime;

namespace nodejs.Http;

/// <summary>
/// Implements Node.js http.Server functionality using Kestrel.
/// Extends EventEmitter to support events like 'request', 'connection', 'close', etc.
/// </summary>
#pragma warning disable ASPDEPR004 // WebHostBuilder deprecation
#pragma warning disable ASPDEPR008 // IWebHost deprecation
public partial class Server : EventEmitter
{
    private IWebHost? _host;
    private readonly Action<IncomingMessage, ServerResponse>? _requestListener;
    private int _maxHeadersCount = 2000;
    private double _timeout = 0; // 0 means no timeout (Node.js default)
    private double _headersTimeout = 60000; // 60 seconds (Node.js default)
    private double _requestTimeout = 300000; // 300 seconds (5 minutes, Node.js default)
    private double _keepAliveTimeout = 5000; // 5 seconds (Node.js default)
    private bool _listening = false;

    private static IPAddress ResolveHostname(string hostname)
    {
        if (IPAddress.TryParse(hostname, out var parsed))
        {
            return parsed;
        }

        if (string.Equals(hostname, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return IPAddress.Loopback;
        }

        var addresses = Dns.GetHostAddresses(hostname);
        if (addresses.Length == 0)
        {
            throw new InvalidOperationException($"Unable to resolve hostname: {hostname}");
        }

        return addresses[0];
    }

    /// <summary>
    /// Creates a new HTTP server.
    /// </summary>
    /// <param name="requestListener">Optional request handler function.</param>
    public Server(Action<IncomingMessage, ServerResponse>? requestListener = null)
    {
        _requestListener = requestListener;

        // If request listener provided, register it as event listener
        if (requestListener != null)
        {
            on("request", requestListener);
        }
    }

    /// <summary>
    /// Limits maximum incoming headers count.
    /// If set to 0, no limit will be applied.
    /// </summary>
    public int maxHeadersCount
    {
        get => _maxHeadersCount;
        set => _maxHeadersCount = value;
    }

    /// <summary>
    /// Sets the timeout value in milliseconds for receiving the entire request from the client.
    /// Default: 0 (no timeout)
    /// </summary>
    public double timeout
    {
        get => _timeout;
        set => _timeout = JsNumeric.RequireFiniteNonNegative(value, nameof(timeout));
    }

    /// <summary>
    /// Limits the amount of time the parser will wait to receive the complete HTTP headers.
    /// Default: 60000 (60 seconds)
    /// </summary>
    public double headersTimeout
    {
        get => _headersTimeout;
        set => _headersTimeout = JsNumeric.RequireFiniteNonNegative(value, nameof(headersTimeout));
    }

    /// <summary>
    /// Sets the timeout value in milliseconds for receiving the entire request from the client.
    /// Default: 300000 (5 minutes)
    /// </summary>
    public double requestTimeout
    {
        get => _requestTimeout;
        set => _requestTimeout = JsNumeric.RequireFiniteNonNegative(value, nameof(requestTimeout));
    }

    /// <summary>
    /// The number of milliseconds of inactivity a server needs to wait for additional data after it has finished writing the last response,
    /// before a socket will be destroyed.
    /// Default: 5000 (5 seconds)
    /// </summary>
    public double keepAliveTimeout
    {
        get => _keepAliveTimeout;
        set => _keepAliveTimeout = JsNumeric.RequireFiniteNonNegative(value, nameof(keepAliveTimeout));
    }

    /// <summary>
    /// Indicates whether or not the server is listening for connections.
    /// </summary>
    public bool listening => _listening;

    /// <summary>
    /// Begin accepting connections on the specified port and hostname.
    /// </summary>
    /// <param name="port">The port number.</param>
    /// <param name="hostname">The hostname. Default: all interfaces</param>
    /// <param name="backlog">Maximum length of the queue of pending connections (ignored in Kestrel).</param>
    /// <param name="callback">Optional callback when server has been started.</param>
    /// <returns>The server instance for chaining.</returns>
    public Server listen(double port, string? hostname = null, double? backlog = null, Action? callback = null)
    {
        if (_listening)
        {
            throw new InvalidOperationException("Server is already listening");
        }

        var normalizedPort = JsNumeric.ToPort(port, nameof(port));
        if (backlog.HasValue)
        {
            JsNumeric.RequireFiniteNonNegative(backlog.Value, nameof(backlog));
        }

        // Use minimal WebHost setup to avoid file watchers
        var host = new WebHostBuilder()
            .UseKestrel(options =>
            {
                if (string.IsNullOrEmpty(hostname))
                {
                    // Listen on all interfaces
                    options.ListenAnyIP(normalizedPort, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1;
                    });
                }
                else
                {
                    // Listen on specific hostname
                    options.Listen(ResolveHostname(hostname), normalizedPort, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1;
                    });
                }

                // Configure limits
                options.Limits.MaxRequestHeaderCount = _maxHeadersCount;
                options.Limits.MaxRequestHeadersTotalSize = http.maxHeaderSize;
                options.Limits.KeepAliveTimeout = TimeSpan.FromMilliseconds(_keepAliveTimeout);
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMilliseconds(_headersTimeout);
            })
            .SuppressStatusMessages(true)
            .Configure(app =>
            {
                // Main request handler - use async to properly await response writes
                app.Run(async context =>
                {
                    var req = new IncomingMessage(context.Request);
                    var res = new ServerResponse(context.Response);

                    // Emit 'request' event - registered listeners will handle the request
                    emit("request", req, res);

                    await context.Response.CompleteAsync();
                });
            })
            .Build();

        _host = host;
        ProcessKeepAlive.Acquire();

        try
        {
            _host.Start();
            _listening = true;
            emit("listening");
            callback?.Invoke();
            return this;
        }
        catch
        {
            _host.Dispose();
            _host = null;
            ProcessKeepAlive.Release();
            throw;
        }
    }

    /// <summary>
    /// Begin accepting connections on the specified port.
    /// </summary>
    /// <param name="port">The port number.</param>
    /// <param name="callback">Optional callback when server has been started.</param>
    /// <returns>The server instance for chaining.</returns>
    public Server listen(double port, Action? callback)
    {
        return listen(port, null, null, callback);
    }

    /// <summary>
    /// Begin accepting connections on the specified port and hostname.
    /// </summary>
    public Server listen(double port, string hostname, Action? callback)
    {
        return listen(port, hostname, null, callback);
    }

    /// <summary>
    /// Stops the server from accepting new connections.
    /// </summary>
    /// <param name="callback">Optional callback when server has closed.</param>
    /// <returns>The server instance for chaining.</returns>
    public Server close(Action? callback = null)
    {
        _ = closeAsync(callback);
        return this;
    }

    private async Task closeAsync(Action? callback)
    {
        if (_host != null)
        {
            try
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            finally
            {
                _host = null;
                _listening = false;
                ProcessKeepAlive.Release();
                emit("close");
                callback?.Invoke();
            }
        }
    }

    /// <summary>
    /// Sets the timeout value for sockets and emits a 'timeout' event on the Server object.
    /// </summary>
    /// <param name="msecs">Timeout in milliseconds.</param>
    /// <param name="callback">Optional callback to be added as a listener on the 'timeout' event.</param>
    /// <returns>The server instance for chaining.</returns>
    public Server setTimeout(double msecs, Action? callback = null)
    {
        _timeout = JsNumeric.RequireFiniteNonNegative(msecs, nameof(msecs));

        if (callback != null)
        {
            on("timeout", callback);
        }

        return this;
    }

    /// <summary>
    /// Returns the bound address, the address family name, and port of the server.
    /// Only useful after 'listening' event.
    /// </summary>
    /// <returns>An object with 'port', 'family', and 'address' properties.</returns>
    public AddressInfo? address()
    {
        // In Kestrel, we don't have easy access to the bound address
        // This would require storing the listen parameters
        // For now, return null if not implemented
        return null;
    }
}

/// <summary>
/// Information about a server's bound address.
/// </summary>
public class AddressInfo
{
    /// <summary>
    /// The port number the server is listening on.
    /// </summary>
    public int port { get; set; }

    /// <summary>
    /// The address family (e.g., "IPv4" or "IPv6").
    /// </summary>
    public string family { get; set; } = "IPv4";

    /// <summary>
    /// The IP address the server is listening on.
    /// </summary>
    public string address { get; set; } = "";
}
