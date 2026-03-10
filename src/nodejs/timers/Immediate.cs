using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly CancellationTokenSource _cancellation = new();
    private bool _isRef = true;
    private bool _disposed = false;

    internal Immediate(Action callback)
    {
        _handleId = Interlocked.Increment(ref _nextHandleId);
        _callback = callback;
        ProcessKeepAlive.Acquire();
        ActiveHandles[_handleId] = this;
        _ = BackgroundDispatch.RunAsync(ExecuteWhenReadyAsync, $"nodejs.Immediate#{_handleId}");
    }

    private async Task ExecuteWhenReadyAsync()
    {
        try
        {
            await Task.Delay(1, _cancellation.Token);

            if (_cancellation.IsCancellationRequested || Volatile.Read(ref _disposed))
            {
                return;
            }

            _callback();
        }
        catch (OperationCanceledException)
        {
            // clearImmediate cancelled execution before the deferred callback ran.
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
            _cancellation.Cancel();
            ActiveHandles.TryRemove(_handleId, out _);
            if (_isRef)
            {
                ProcessKeepAlive.Release();
            }
        }
        GC.SuppressFinalize(this);
    }
}
