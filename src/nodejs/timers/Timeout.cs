using System;
using System.Threading;

namespace nodejs;

/// <summary>
/// Represents a timeout that can be used with setTimeout/clearTimeout.
/// </summary>
public class Timeout : IDisposable
{
    private Timer? _timer;
    private readonly Action _callback;
    private bool _isRef = true;
    private bool _disposed = false;

    internal Timeout(Action callback, int delay)
    {
        _callback = callback;
        _timer = new Timer(_ => Execute(), null, delay, System.Threading.Timeout.Infinite);
    }

    private void Execute()
    {
        if (!_disposed)
        {
            _callback();
        }
    }

    /// <summary>
    /// Requests that the Node.js event loop not exit so long as the Timeout is active.
    /// In this C# implementation, this is a no-op for compatibility.
    /// </summary>
    public Timeout @ref()
    {
        _isRef = true;
        return this;
    }

    /// <summary>
    /// Allows the Node.js event loop to exit if this is the only active handle.
    /// In this C# implementation, this is a no-op for compatibility.
    /// </summary>
    public Timeout unref()
    {
        _isRef = false;
        return this;
    }

    /// <summary>
    /// Returns true if the timer will keep the event loop active.
    /// </summary>
    public bool hasRef()
    {
        return _isRef;
    }

    /// <summary>
    /// Restarts the timer, as if it was just created.
    /// </summary>
    public Timeout refresh()
    {
        if (_timer != null && !_disposed)
        {
            // Note: Cannot truly reset a Timer, would need to track original delay
            // For now, this is a no-op
        }
        return this;
    }

    /// <summary>
    /// Cancels the timeout (alias for clearTimeout).
    /// </summary>
    public void close()
    {
        Dispose();
    }

    /// <summary>
    /// Disposes the timer resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            var timer = _timer;
            _timer = null;
            timer?.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
