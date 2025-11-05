using Xunit;
using System.Threading;

namespace Tsonic.Node.Tests;

public class TimersTests
{
    [Fact]
    public void setTimeout_ShouldExecuteCallback()
    {
        var executed = false;
        var timeout = timers.setTimeout(() => executed = true, 50);

        Thread.Sleep(100);
        Assert.True(executed);
    }

    [Fact]
    public void setTimeout_ShouldReturnTimeout()
    {
        var timeout = timers.setTimeout(() => { }, 10);
        Assert.NotNull(timeout);
        Assert.IsType<Timeout>(timeout);
    }

    [Fact]
    public void setTimeout_WithZeroDelay_ShouldExecute()
    {
        var executed = false;
        timers.setTimeout(() => executed = true, 0);

        Thread.Sleep(50);
        Assert.True(executed);
    }

    [Fact]
    public void clearTimeout_ShouldCancelTimeout()
    {
        var executed = false;
        var timeout = timers.setTimeout(() => executed = true, 50);

        timers.clearTimeout(timeout);
        Thread.Sleep(100);

        Assert.False(executed);
    }

    [Fact]
    public void clearTimeout_WithNull_ShouldNotThrow()
    {
        timers.clearTimeout(null);
        // Should not throw
    }

    [Fact]
    public void setInterval_ShouldExecuteRepeatedlyAsync()
    {
        var count = 0;
        var timeout = timers.setInterval(() => Interlocked.Increment(ref count), 50);

        Thread.Sleep(250);
        timers.clearInterval(timeout);
        Thread.Sleep(50); // Allow pending callbacks to complete

        Assert.True(count >= 3, $"Expected at least 3 executions, got {count}");
    }

    [Fact]
    public void clearInterval_ShouldNotThrow()
    {
        var count = 0;
        var timeout = timers.setInterval(() => Interlocked.Increment(ref count), 50);

        Thread.Sleep(60);
        timers.clearInterval(timeout);

        // Just verify that clearInterval doesn't throw
        Assert.True(count > 0, "Interval should have executed at least once");
    }

    [Fact]
    public void setImmediate_ShouldExecuteCallback()
    {
        var resetEvent = new ManualResetEventSlim(false);
        var executed = false;
        var immediate = timers.setImmediate(() =>
        {
            executed = true;
            resetEvent.Set();
        });

        var signaled = resetEvent.Wait(1000);
        Assert.True(signaled, "setImmediate callback was not called within timeout");
        Assert.True(executed);
    }

    [Fact]
    public void setImmediate_ShouldReturnImmediate()
    {
        var immediate = timers.setImmediate(() => { });
        Assert.NotNull(immediate);
        Assert.IsType<Immediate>(immediate);
    }

    [Fact]
    public void clearImmediate_ShouldCancelImmediate()
    {
        var executed = false;
        var immediate = timers.setImmediate(() => executed = true);

        timers.clearImmediate(immediate);
        Thread.Sleep(100);

        Assert.False(executed);
    }

    [Fact]
    public void clearImmediate_WithNull_ShouldNotThrow()
    {
        timers.clearImmediate(null);
        // Should not throw
    }

    [Fact]
    public void queueMicrotask_ShouldExecuteCallback()
    {
        var executed = false;
        timers.queueMicrotask(() => executed = true);

        Thread.Sleep(100);
        Assert.True(executed);
    }

    [Fact]
    public void Timeout_ref_ShouldReturnThis()
    {
        var timeout = timers.setTimeout(() => { }, 100);
        var result = timeout.@ref();

        Assert.Same(timeout, result);
        timers.clearTimeout(timeout);
    }

    [Fact]
    public void Timeout_unref_ShouldReturnThis()
    {
        var timeout = timers.setTimeout(() => { }, 100);
        var result = timeout.unref();

        Assert.Same(timeout, result);
        timers.clearTimeout(timeout);
    }

    [Fact]
    public void Timeout_hasRef_ShouldReturnTrue()
    {
        var timeout = timers.setTimeout(() => { }, 100);
        Assert.True(timeout.hasRef());
        timers.clearTimeout(timeout);
    }

    [Fact]
    public void Timeout_hasRef_AfterUnref_ShouldReturnFalse()
    {
        var timeout = timers.setTimeout(() => { }, 100);
        timeout.unref();
        Assert.False(timeout.hasRef());
        timers.clearTimeout(timeout);
    }

    [Fact]
    public void Timeout_refresh_ShouldReturnThis()
    {
        var timeout = timers.setTimeout(() => { }, 100);
        var result = timeout.refresh();

        Assert.Same(timeout, result);
        timers.clearTimeout(timeout);
    }

    [Fact]
    public void Timeout_close_ShouldCancelTimeout()
    {
        var executed = false;
        var timeout = timers.setTimeout(() => executed = true, 50);

        timeout.close();
        Thread.Sleep(100);

        Assert.False(executed);
    }

    [Fact]
    public void Immediate_ref_ShouldReturnThis()
    {
        var immediate = timers.setImmediate(() => { });
        var result = immediate.@ref();

        Assert.Same(immediate, result);
        timers.clearImmediate(immediate);
    }

    [Fact]
    public void Immediate_unref_ShouldReturnThis()
    {
        var immediate = timers.setImmediate(() => { });
        var result = immediate.unref();

        Assert.Same(immediate, result);
        timers.clearImmediate(immediate);
    }

    [Fact]
    public void Immediate_hasRef_ShouldReturnTrue()
    {
        var immediate = timers.setImmediate(() => { });
        Assert.True(immediate.hasRef());
        timers.clearImmediate(immediate);
    }

    [Fact]
    public void Immediate_hasRef_AfterUnref_ShouldReturnFalse()
    {
        var immediate = timers.setImmediate(() => { });
        immediate.unref();
        Assert.False(immediate.hasRef());
        timers.clearImmediate(immediate);
    }
}
