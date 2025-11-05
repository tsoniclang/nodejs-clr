using System;
using System.Text;
using System.Threading;
using Xunit;

namespace Tsonic.Node.Tests;

public class DgramTests
{
    [Fact]
    public void CreateSocket_UDP4_CreatesSocket()
    {
        var socket = dgram.createSocket("udp4");
        Assert.NotNull(socket);
        socket.close();
    }

    [Fact]
    public void CreateSocket_UDP6_CreatesSocket()
    {
        var socket = dgram.createSocket("udp6");
        Assert.NotNull(socket);
        socket.close();
    }

    [Fact]
    public void CreateSocket_WithCallback_AttachesMessageListener()
    {
        var messageReceived = false;
        byte[]? receivedData = null;
        RemoteInfo? receivedInfo = null;

        var socket = dgram.createSocket("udp4", (data, rinfo) =>
        {
            messageReceived = true;
            receivedData = data;
            receivedInfo = rinfo;
        });

        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100); // Wait for bind

        var addr = socket.address();
        var client = dgram.createSocket("udp4");

        client.send("test", addr.port, "127.0.0.1", (err, bytes) =>
        {
            client.close();
        });

        Thread.Sleep(200); // Wait for message

        Assert.True(messageReceived);
        Assert.NotNull(receivedData);
        Assert.Equal("test", Encoding.UTF8.GetString(receivedData!));
        Assert.NotNull(receivedInfo);
        Assert.Equal("127.0.0.1", receivedInfo!.address);

        socket.close();
    }

    [Fact]
    public void Bind_WithPort_BindsSuccessfully()
    {
        var socket = dgram.createSocket("udp4");
        var listening = false;

        socket.on("listening", (Action)(() =>
        {
            listening = true;
        }));

        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        Assert.True(listening);
        var addr = socket.address();
        Assert.Equal("127.0.0.1", addr.address);
        Assert.True(addr.port > 0);

        socket.close();
    }

    [Fact]
    public void Bind_WithCallback_CallsCallback()
    {
        var socket = dgram.createSocket("udp4");
        var callbackCalled = false;

        socket.bind(0, "127.0.0.1", () =>
        {
            callbackCalled = true;
        });

        Thread.Sleep(100);
        Assert.True(callbackCalled);

        socket.close();
    }

    [Fact]
    public void Send_ToRemote_SendsMessage()
    {
        var server = dgram.createSocket("udp4");
        var messageReceived = false;
        byte[]? receivedData = null;

        server.on("message", (Action<byte[], RemoteInfo>)((data, rinfo) =>
        {
            messageReceived = true;
            receivedData = data;
        }));

        server.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var addr = server.address();
        var client = dgram.createSocket("udp4");

        var testMessage = "Hello UDP";
        client.send(testMessage, addr.port, "127.0.0.1");

        Thread.Sleep(200);

        Assert.True(messageReceived);
        Assert.NotNull(receivedData);
        Assert.Equal(testMessage, Encoding.UTF8.GetString(receivedData!));

        client.close();
        server.close();
    }

    [Fact]
    public void Send_WithCallback_CallsCallback()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var addr = socket.address();
        var client = dgram.createSocket("udp4");

        var callbackCalled = false;
        Exception? callbackError = null;
        int bytesSent = 0;

        client.send("test", addr.port, "127.0.0.1", (err, bytes) =>
        {
            callbackCalled = true;
            callbackError = err;
            bytesSent = bytes;
        });

        Thread.Sleep(200);

        Assert.True(callbackCalled);
        Assert.Null(callbackError);
        Assert.Equal(4, bytesSent);

        client.close();
        socket.close();
    }

    [Fact]
    public void Connect_ToRemote_ConnectsSuccessfully()
    {
        var server = dgram.createSocket("udp4");
        server.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var addr = server.address();
        var client = dgram.createSocket("udp4");

        var connected = false;
        client.on("connect", (Action)(() =>
        {
            connected = true;
        }));

        client.connect(addr.port, "127.0.0.1");
        Thread.Sleep(100);

        Assert.True(connected);

        var remoteAddr = client.remoteAddress();
        Assert.Equal("127.0.0.1", remoteAddr.address);
        Assert.Equal(addr.port, remoteAddr.port);

        client.close();
        server.close();
    }

    [Fact]
    public void Send_ConnectedSocket_SendsWithoutAddress()
    {
        var server = dgram.createSocket("udp4");
        var messageReceived = false;

        server.on("message", (Action<byte[], RemoteInfo>)((data, rinfo) =>
        {
            messageReceived = true;
        }));

        server.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var addr = server.address();
        var client = dgram.createSocket("udp4");
        client.connect(addr.port, "127.0.0.1");
        Thread.Sleep(100);

        client.send("test");
        Thread.Sleep(200);

        Assert.True(messageReceived);

        client.close();
        server.close();
    }

    [Fact]
    public void Disconnect_ConnectedSocket_Disconnects()
    {
        var server = dgram.createSocket("udp4");
        server.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var addr = server.address();
        var client = dgram.createSocket("udp4");
        client.connect(addr.port, "127.0.0.1");
        Thread.Sleep(100);

        // Should not throw
        client.disconnect();

        // Should throw after disconnect
        Assert.Throws<InvalidOperationException>(() => client.remoteAddress());

        client.close();
        server.close();
    }

    [Fact]
    public void Close_BoundSocket_ClosesSuccessfully()
    {
        var socket = dgram.createSocket("udp4");
        var closed = false;

        socket.on("close", (Action)(() =>
        {
            closed = true;
        }));

        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        socket.close();
        Thread.Sleep(100);

        Assert.True(closed);
    }

    [Fact]
    public void SetBroadcast_EnablesBroadcast()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        // Should not throw
        socket.setBroadcast(true);
        socket.setBroadcast(false);

        socket.close();
    }

    [Fact]
    public void SetMulticastTTL_SetsTTL()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var result = socket.setMulticastTTL(128);
        Assert.Equal(128, result);

        socket.close();
    }

    [Fact]
    public void SetMulticastLoopback_SetsLoopback()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var result = socket.setMulticastLoopback(true);
        Assert.True(result);

        socket.close();
    }

    [Fact]
    public void AddMembership_JoinsMulticastGroup()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "0.0.0.0");
        Thread.Sleep(100);

        // Should not throw
        socket.addMembership("224.0.0.1");
        socket.dropMembership("224.0.0.1");

        socket.close();
    }

    [Fact]
    public void SetRecvBufferSize_SetsBufferSize()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        socket.setRecvBufferSize(8192);
        var size = socket.getRecvBufferSize();
        // Buffer size might be adjusted by OS
        Assert.True(size > 0);

        socket.close();
    }

    [Fact]
    public void SetSendBufferSize_SetsBufferSize()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        socket.setSendBufferSize(8192);
        var size = socket.getSendBufferSize();
        // Buffer size might be adjusted by OS
        Assert.True(size > 0);

        socket.close();
    }

    [Fact]
    public void Address_UnboundSocket_ThrowsException()
    {
        var socket = dgram.createSocket("udp4");
        Assert.Throws<InvalidOperationException>(() => socket.address());
        socket.close();
    }

    [Fact]
    public void RemoteAddress_UnconnectedSocket_ThrowsException()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        Assert.Throws<InvalidOperationException>(() => socket.remoteAddress());
        socket.close();
    }

    [Fact]
    public void Send_WithOffsetAndLength_SendsCorrectSlice()
    {
        var server = dgram.createSocket("udp4");
        byte[]? receivedData = null;

        server.on("message", (Action<byte[], RemoteInfo>)((data, rinfo) =>
        {
            receivedData = data;
        }));

        server.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var addr = server.address();
        var client = dgram.createSocket("udp4");

        var buffer = Encoding.UTF8.GetBytes("HelloWorld");
        client.send(buffer, 5, 5, addr.port, "127.0.0.1");

        Thread.Sleep(200);

        Assert.NotNull(receivedData);
        Assert.Equal("World", Encoding.UTF8.GetString(receivedData!));

        client.close();
        server.close();
    }

    [Fact]
    public void Send_WithInvalidOffset_ThrowsException()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var buffer = new byte[10];
        Assert.Throws<ArgumentOutOfRangeException>(() => socket.send(buffer, 15, 5, 1234, "127.0.0.1"));

        socket.close();
    }

    [Fact]
    public void Send_WithInvalidLength_ThrowsException()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var buffer = new byte[10];
        Assert.Throws<ArgumentOutOfRangeException>(() => socket.send(buffer, 5, 10, 1234, "127.0.0.1"));

        socket.close();
    }

    [Fact]
    public void SetTTL_SetsTTL()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var result = socket.setTTL(128);
        Assert.Equal(128, result);

        socket.close();
    }

    [Fact]
    public void SetTTL_InvalidValue_ThrowsException()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        Assert.Throws<ArgumentException>(() => socket.setTTL(0));
        Assert.Throws<ArgumentException>(() => socket.setTTL(256));

        socket.close();
    }

    [Fact]
    public void GetSendQueueSize_ReturnsZero()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var size = socket.getSendQueueSize();
        Assert.Equal(0, size);

        socket.close();
    }

    [Fact]
    public void GetSendQueueCount_ReturnsZero()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var count = socket.getSendQueueCount();
        Assert.Equal(0, count);

        socket.close();
    }

    [Fact]
    public void Ref_ReturnsSocket()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var result = socket.@ref();
        Assert.Same(socket, result);

        socket.close();
    }

    [Fact]
    public void Unref_ReturnsSocket()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var result = socket.unref();
        Assert.Same(socket, result);

        socket.close();
    }

    [Fact]
    public void AddSourceSpecificMembership_ThrowsNotSupported()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "0.0.0.0");
        Thread.Sleep(100);

        Assert.Throws<NotSupportedException>(() => socket.addSourceSpecificMembership("192.168.1.1", "224.0.0.1"));

        socket.close();
    }

    [Fact]
    public void DropSourceSpecificMembership_ThrowsNotSupported()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "0.0.0.0");
        Thread.Sleep(100);

        Assert.Throws<NotSupportedException>(() => socket.dropSourceSpecificMembership("192.168.1.1", "224.0.0.1"));

        socket.close();
    }

    [Fact]
    public void DropMembership_LeavesMulticastGroup()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "0.0.0.0");
        Thread.Sleep(100);

        // Add membership first
        socket.addMembership("224.0.0.1");

        // Then drop it - should not throw
        socket.dropMembership("224.0.0.1");

        socket.close();
    }

    [Fact]
    public void GetRecvBufferSize_ReturnsBufferSize()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var size = socket.getRecvBufferSize();
        Assert.True(size > 0);

        socket.close();
    }

    [Fact]
    public void GetSendBufferSize_ReturnsBufferSize()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "127.0.0.1");
        Thread.Sleep(100);

        var size = socket.getSendBufferSize();
        Assert.True(size > 0);

        socket.close();
    }

    [Fact]
    public void SetMulticastInterface_SetsInterface()
    {
        var socket = dgram.createSocket("udp4");
        socket.bind(0, "0.0.0.0");
        Thread.Sleep(100);

        // Should not throw when setting a valid local interface IP
        socket.setMulticastInterface("127.0.0.1");

        socket.close();
    }

    [Fact]
    public void Bind_WithBindOptions_BindsSuccessfully()
    {
        var socket = dgram.createSocket("udp4");

        var options = new BindOptions
        {
            port = 0,
            address = "127.0.0.1"
        };

        socket.bind(options);
        Thread.Sleep(100);

        var addr = socket.address();
        Assert.Equal("127.0.0.1", addr.address);
        Assert.True(addr.port > 0);

        socket.close();
    }

    [Fact]
    public void Bind_WithBindOptionsAndCallback_CallsCallback()
    {
        var socket = dgram.createSocket("udp4");
        bool callbackCalled = false;

        var options = new BindOptions
        {
            port = 0,
            address = "127.0.0.1"
        };

        socket.bind(options, () => { callbackCalled = true; });
        Thread.Sleep(100);

        Assert.True(callbackCalled);

        socket.close();
    }

    [Fact]
    public void Bind_WithFileDescriptor_ThrowsNotSupported()
    {
        var socket = dgram.createSocket("udp4");

        var options = new BindOptions
        {
            fd = 123
        };

        Assert.Throws<NotSupportedException>(() => socket.bind(options));

        socket.close();
    }
}
