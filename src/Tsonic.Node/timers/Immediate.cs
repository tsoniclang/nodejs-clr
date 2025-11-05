using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tsonic.Node;

/// <summary>
/// Represents an immediate callback that can be used with setImmediate/clearImmediate.
/// </summary>
public class Immediate : IDisposable
{
    private CancellationTokenSource? _cts;
    private readonly Action _callback;
    private bool _isRef = true;
    private bool _disposed = false;

    internal Immediate(Action callback)
    {
        _callback = callback;
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        Task.Run(() =>
        {
            if (!token.IsCancellationRequested && !_disposed)
            {
                _callback();
            }
        }, token);
    }

    /// <summary>
    /// Requests that the Node.js event loop not exit so long as the Immediate is active.
    /// In this C# implementation, this is a no-op for compatibility.
    /// </summary>
    public Immediate @ref()
    {
        _isRef = true;
        return this;
    }

    /// <summary>
    /// Allows the Node.js event loop to exit if this is the only active handle.
    /// In this C# implementation, this is a no-op for compatibility.
    /// </summary>
    public Immediate unref()
    {
        _isRef = false;
        return this;
    }

    /// <summary>
    /// Returns true if the immediate will keep the event loop active.
    /// </summary>
    public bool hasRef()
    {
        return _isRef;
    }

    /// <summary>
    /// Disposes the immediate resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
        GC.SuppressFinalize(this);
    }
}
