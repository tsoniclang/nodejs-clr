using System;
using System.Threading;
using Xunit;

namespace Tsonic.Node.Tests;

public class SocketTests
{
    private const int TEST_PORT = 18234;

    [Fact]
    public void Socket_Constructor_CreatesInstance()
    {
        var socket = new Socket();
        Assert.NotNull(socket);
        Assert.Equal("closed", socket.readyState);
    }

    [Fact]
    public void Socket_ConstructorWithOptions_CreatesInstance()
    {
        var options = new SocketConstructorOpts { allowHalfOpen = true };
        var socket = new Socket(options);
        Assert.NotNull(socket);
    }

    [Fact]
    public void Socket_BytesRead_InitiallyZero()
    {
        var socket = new Socket();
        Assert.Equal(0, socket.bytesRead);
    }

    [Fact]
    public void Socket_BytesWritten_InitiallyZero()
    {
        var socket = new Socket();
        Assert.Equal(0, socket.bytesWritten);
    }

    [Fact]
    public void Socket_Connecting_InitiallyFalse()
    {
        var socket = new Socket();
        Assert.False(socket.connecting);
    }

    [Fact]
    public void Socket_Destroyed_InitiallyFalse()
    {
        var socket = new Socket();
        Assert.False(socket.destroyed);
    }

    [Fact]
    public void Socket_ReadyState_InitiallyClosed()
    {
        var socket = new Socket();
        Assert.Equal("closed", socket.readyState);
    }

    [Fact]
    public void Socket_Connect_StartsConnection()
    {
        var socket = new Socket();
        var resetEvent = new ManualResetEventSlim(false);

        socket.on("error", (Exception err) =>
        {
            resetEvent.Set();
        });

        socket.connect(TEST_PORT, "localhost", () =>
        {
            resetEvent.Set();
        });

        // Should be connecting
        Assert.True(socket.connecting || socket.readyState == "opening");

        resetEvent.Wait(2000);
        // Connection will likely fail since no server is running, that's okay
    }

    [Fact]
    public void Socket_Destroy_MarksAsDestroyed()
    {
        var socket = new Socket();
        socket.destroy();
        Assert.True(socket.destroyed);
        Assert.Equal("closed", socket.readyState);
    }

    [Fact]
    public void Socket_Destroy_EmitsCloseEvent()
    {
        var socket = new Socket();
        var closeEmitted = false;
        var resetEvent = new ManualResetEventSlim(false);

        socket.on("close", (bool hadError) =>
        {
            closeEmitted = true;
            resetEvent.Set();
        });

        socket.destroy();

        resetEvent.Wait(1000);
        Assert.True(closeEmitted);
    }

    [Fact]
    public void Socket_DestroyWithError_EmitsError()
    {
        var socket = new Socket();
        var errorEmitted = false;
        var resetEvent = new ManualResetEventSlim(false);

        socket.on("error", (Exception err) =>
        {
            errorEmitted = true;
            resetEvent.Set();
        });

        socket.destroy(new Exception("Test error"));

        resetEvent.Wait(1000);
        Assert.True(errorEmitted);
    }

    [Fact]
    public void Socket_SetTimeout_DoesNotThrow()
    {
        var socket = new Socket();
        var exception = Record.Exception(() =>
        {
            socket.setTimeout(5000);
        });
        Assert.Null(exception);
    }

    [Fact]
    public void Socket_SetNoDelay_DoesNotThrow()
    {
        var socket = new Socket();
        var exception = Record.Exception(() =>
        {
            socket.setNoDelay(true);
        });
        Assert.Null(exception);
    }

    [Fact]
    public void Socket_SetKeepAlive_DoesNotThrow()
    {
        var socket = new Socket();
        var exception = Record.Exception(() =>
        {
            socket.setKeepAlive(true, 1000);
        });
        Assert.Null(exception);
    }

    [Fact]
    public void Socket_Address_ReturnsEmptyObjectWhenNotConnected()
    {
        var socket = new Socket();
        var address = socket.address();
        Assert.NotNull(address);
    }

    [Fact]
    public void Socket_Unref_ReturnsSocket()
    {
        var socket = new Socket();
        var result = socket.unref();
        Assert.Same(socket, result);
    }

    [Fact]
    public void Socket_Ref_ReturnsSocket()
    {
        var socket = new Socket();
        var result = socket.@ref();
        Assert.Same(socket, result);
    }

    [Fact]
    public void Socket_Pause_ReturnsSocket()
    {
        var socket = new Socket();
        var result = socket.pause();
        Assert.Same(socket, result);
    }

    [Fact]
    public void Socket_Resume_ReturnsSocket()
    {
        var socket = new Socket();
        var result = socket.resume();
        Assert.Same(socket, result);
    }

    [Fact]
    public void Socket_SetEncoding_ReturnsSocket()
    {
        var socket = new Socket();
        var result = socket.setEncoding("utf8");
        Assert.Same(socket, result);
    }

    [Fact]
    public void Socket_End_ReturnsSocket()
    {
        var socket = new Socket();
        var result = socket.end();
        Assert.Same(socket, result);
    }

    [Fact]
    public void Socket_ResetAndDestroy_ReturnsSocket()
    {
        var socket = new Socket();
        var result = socket.resetAndDestroy();
        Assert.Same(socket, result);
        Assert.True(socket.destroyed);
    }
}
