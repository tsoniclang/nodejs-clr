using System;
using System.Collections.Concurrent;
using System.Threading;
using Tsonic.JSRuntime;

namespace nodejs;

/// <summary>
/// Represents an immediate callback that can be used with setImmediate/clearImmediate.
/// </summary>
public class Immediate : IDisposable
{
    private static int _nextHandleId = 0;
    private static readonly ConcurrentDictionary<int, Immediate> ActiveHandles = new();

    private readonly int _handleId;
    private readonly Action _callback;
    private Timer? _timer;
    private bool _isRef = true;
    private bool _disposed = false;

    internal Immediate(Action callback)
    {
        _handleId = Interlocked.Increment(ref _nextHandleId);
        _callback = callback;
        ProcessKeepAlive.Acquire();
        ActiveHandles[_handleId] = this;
        _timer = new Timer(_ => Execute(), null, 1, System.Threading.Timeout.Infinite);
    }

    private void Execute()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            _callback();
        }
        finally
        {
            Dispose();
        }
    }

    /// <summary>
    /// Requests that the Node.js event loop not exit so long as the Immediate is active.
    /// In this C# implementation, this is a no-op for compatibility.
    /// </summary>
    public Immediate @ref()
    {
        if (!_disposed && !_isRef)
        {
            ProcessKeepAlive.Acquire();
        }
        _isRef = true;
        return this;
    }

    /// <summary>
    /// Allows the Node.js event loop to exit if this is the only active handle.
    /// In this C# implementation, this is a no-op for compatibility.
    /// </summary>
    public Immediate unref()
    {
        if (!_disposed && _isRef)
        {
            ProcessKeepAlive.Release();
        }
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
            var timer = _timer;
            _timer = null;
            ActiveHandles.TryRemove(_handleId, out _);
            timer?.Dispose();
            if (_isRef)
            {
                ProcessKeepAlive.Release();
            }
        }
        GC.SuppressFinalize(this);
    }
}
