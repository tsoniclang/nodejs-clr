using System;
using System.Threading;
using Xunit;

namespace nodejs.Tests;

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

    [Fact]
    public void Socket_DataEvent_EmitsReceivedData()
    {
        var dataReceived = false;
        var receivedMessage = "";
        var resetEvent = new ManualResetEventSlim(false);
        var testMessage = "Hello, Socket!";

        // Create a server
        var server = net.createServer((Socket clientSocket) =>
        {
            clientSocket.on("data", (Buffer data) =>
            {
                dataReceived = true;
                receivedMessage = data.toString();
                resetEvent.Set();
            });
        });

        server.listen(TEST_PORT + 1, () =>
        {
            // Connect a client and send data
            var client = new Socket();
            client.connect(TEST_PORT + 1, "localhost", () =>
            {
                client.write(testMessage);
            });
        });

        // Wait for data event
        var signaled = resetEvent.Wait(5000);
        server.close();

        Assert.True(signaled, "Data event should have been received within timeout");
        Assert.True(dataReceived, "Data event should have been emitted");
        Assert.Equal(testMessage, receivedMessage);
    }

    [Fact]
    public void Socket_DataEvent_EmitsMultipleChunks()
    {
        var dataChunks = new System.Collections.Generic.List<string>();
        var resetEvent = new ManualResetEventSlim(false);

        // Create a server
        var server = net.createServer((Socket clientSocket) =>
        {
            clientSocket.on("data", (Buffer data) =>
            {
                dataChunks.Add(data.toString());
                if (dataChunks.Count >= 3)
                {
                    resetEvent.Set();
                }
            });
        });

        server.listen(TEST_PORT + 2, () =>
        {
            // Connect a client and send multiple chunks
            var client = new Socket();
            client.connect(TEST_PORT + 2, "localhost", () =>
            {
                client.write("Chunk1");
                Thread.Sleep(50);
                client.write("Chunk2");
                Thread.Sleep(50);
                client.write("Chunk3");
            });
        });

        // Wait for all data events
        var signaled = resetEvent.Wait(5000);
        server.close();

        Assert.True(signaled, "Should have received 3 data chunks within timeout");
        Assert.True(dataChunks.Count >= 3, "Should have received at least 3 data events");
    }

    [Fact]
    public void Socket_EndEvent_EmitsOnConnectionClose()
    {
        var endReceived = false;
        var resetEvent = new ManualResetEventSlim(false);

        // Create a server
        var server = net.createServer((Socket clientSocket) =>
        {
            clientSocket.on("end", () =>
            {
                endReceived = true;
                resetEvent.Set();
            });
        });

        server.listen(TEST_PORT + 3, () =>
        {
            // Connect a client, then close it
            var client = new Socket();
            client.connect(TEST_PORT + 3, "localhost", () =>
            {
                Thread.Sleep(100);
                client.end();
            });
        });

        // Wait for end event
        var signaled = resetEvent.Wait(5000);
        server.close();

        Assert.True(signaled, "End event should have been received within timeout");
        Assert.True(endReceived, "End event should have been emitted");
    }

    [Fact]
    public void Socket_WriteAndReceive_RoundTrip()
    {
        var serverReceived = "";
        var clientReceived = "";
        var resetEvent = new ManualResetEventSlim(false);

        // Create a server that echoes data
        var server = net.createServer((Socket clientSocket) =>
        {
            clientSocket.on("data", (Buffer data) =>
            {
                serverReceived = data.toString();
                // Echo back with a prefix
                clientSocket.write("Echo: " + serverReceived);
            });
        });

        server.listen(TEST_PORT + 4, () =>
        {
            var client = new Socket();
            client.on("data", (Buffer data) =>
            {
                clientReceived = data.toString();
                resetEvent.Set();
            });

            client.connect(TEST_PORT + 4, "localhost", () =>
            {
                client.write("Test message");
            });
        });

        // Wait for round-trip
        var signaled = resetEvent.Wait(5000);
        server.close();

        Assert.True(signaled, "Should have completed round-trip within timeout");
        Assert.Equal("Test message", serverReceived);
        Assert.Equal("Echo: Test message", clientReceived);
    }
}
