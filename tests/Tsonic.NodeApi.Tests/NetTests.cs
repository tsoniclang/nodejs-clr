using System;
using System.Threading;
using System.Linq;
using Xunit;
using Tsonic.NodeApi;

namespace Tsonic.NodeApi.Tests;

public class NetTests
{
    private const int TEST_PORT = 18234; // Use a high port number to avoid conflicts

    // ==================== Static net Methods Tests ====================

    [Fact]
    public void CreateServer_NoArgs_ReturnsServer()
    {
        var server = net.createServer();
        Assert.NotNull(server);
        Assert.IsType<Server>(server);
    }

    [Fact]
    public void CreateServer_WithConnectionListener_AttachesListener()
    {
        var server = net.createServer((socket) =>
        {
            // Listener attached
        });
        Assert.NotNull(server);
        // Listener will be called when a connection is made
    }

    [Fact]
    public void CreateServer_WithOptions_ReturnsServer()
    {
        var options = new ServerOpts { allowHalfOpen = true };
        var server = net.createServer(options);
        Assert.NotNull(server);
    }

    [Fact]
    public void Connect_CreatesSocket()
    {
        var socket = net.connect(TEST_PORT, "localhost");
        Assert.NotNull(socket);
        Assert.IsType<Socket>(socket);
    }

    [Fact]
    public void CreateConnection_CreatesSocket()
    {
        var socket = net.createConnection(TEST_PORT, "localhost");
        Assert.NotNull(socket);
        Assert.IsType<Socket>(socket);
    }

    [Fact]
    public void IsIP_ValidIPv4_Returns4()
    {
        Assert.Equal(4, net.isIP("127.0.0.1"));
        Assert.Equal(4, net.isIP("192.168.1.1"));
        Assert.Equal(4, net.isIP("8.8.8.8"));
    }

    [Fact]
    public void IsIP_ValidIPv6_Returns6()
    {
        Assert.Equal(6, net.isIP("::1"));
        Assert.Equal(6, net.isIP("2001:4860:4860::8888"));
        Assert.Equal(6, net.isIP("fe80::1"));
    }

    [Fact]
    public void IsIP_InvalidIP_Returns0()
    {
        Assert.Equal(0, net.isIP("invalid"));
        Assert.Equal(0, net.isIP("999.999.999.999"));
        Assert.Equal(0, net.isIP(""));
        Assert.Equal(0, net.isIP("localhost"));
    }

    [Fact]
    public void IsIPv4_ValidIPv4_ReturnsTrue()
    {
        Assert.True(net.isIPv4("127.0.0.1"));
        Assert.True(net.isIPv4("192.168.1.1"));
    }

    [Fact]
    public void IsIPv4_IPv6_ReturnsFalse()
    {
        Assert.False(net.isIPv4("::1"));
        Assert.False(net.isIPv4("2001:4860:4860::8888"));
    }

    [Fact]
    public void IsIPv4_Invalid_ReturnsFalse()
    {
        Assert.False(net.isIPv4("invalid"));
        Assert.False(net.isIPv4(""));
    }

    [Fact]
    public void IsIPv6_ValidIPv6_ReturnsTrue()
    {
        Assert.True(net.isIPv6("::1"));
        Assert.True(net.isIPv6("2001:4860:4860::8888"));
    }

    [Fact]
    public void IsIPv6_IPv4_ReturnsFalse()
    {
        Assert.False(net.isIPv6("127.0.0.1"));
        Assert.False(net.isIPv6("192.168.1.1"));
    }

    [Fact]
    public void IsIPv6_Invalid_ReturnsFalse()
    {
        Assert.False(net.isIPv6("invalid"));
        Assert.False(net.isIPv6(""));
    }

    [Fact]
    public void GetDefaultAutoSelectFamily_ReturnsBoolean()
    {
        var value = net.getDefaultAutoSelectFamily();
        Assert.IsType<bool>(value);
    }

    [Fact]
    public void SetDefaultAutoSelectFamily_UpdatesValue()
    {
        var original = net.getDefaultAutoSelectFamily();
        net.setDefaultAutoSelectFamily(!original);
        Assert.Equal(!original, net.getDefaultAutoSelectFamily());
        // Reset to original
        net.setDefaultAutoSelectFamily(original);
    }

    [Fact]
    public void GetDefaultAutoSelectFamilyAttemptTimeout_ReturnsInt()
    {
        var value = net.getDefaultAutoSelectFamilyAttemptTimeout();
        Assert.IsType<int>(value);
        Assert.True(value >= 0);
    }

    [Fact]
    public void SetDefaultAutoSelectFamilyAttemptTimeout_UpdatesValue()
    {
        var original = net.getDefaultAutoSelectFamilyAttemptTimeout();
        net.setDefaultAutoSelectFamilyAttemptTimeout(500);
        Assert.Equal(500, net.getDefaultAutoSelectFamilyAttemptTimeout());
        // Reset to original
        net.setDefaultAutoSelectFamilyAttemptTimeout(original);
    }

    // ==================== Socket Class Tests ====================

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

    // ==================== Server Class Tests ====================

    [Fact]
    public void Server_Constructor_CreatesInstance()
    {
        var server = new Server();
        Assert.NotNull(server);
        Assert.False(server.listening);
    }

    [Fact]
    public void Server_ConstructorWithListener_CreatesInstance()
    {
        var server = new Server((socket) => { });
        Assert.NotNull(server);
    }

    [Fact]
    public void Server_ConstructorWithOptions_CreatesInstance()
    {
        var options = new ServerOpts { allowHalfOpen = true };
        var server = new Server(options, null);
        Assert.NotNull(server);
    }

    [Fact]
    public void Server_Listening_InitiallyFalse()
    {
        var server = new Server();
        Assert.False(server.listening);
    }

    [Fact]
    public void Server_MaxConnections_CanBeSet()
    {
        var server = new Server();
        server.maxConnections = 100;
        Assert.Equal(100, server.maxConnections);
    }

    [Fact]
    public void Server_Listen_StartsServer()
    {
        var server = new Server();
        var resetEvent = new ManualResetEventSlim(false);
        var listeningEmitted = false;

        server.on("listening", () =>
        {
            listeningEmitted = true;
            resetEvent.Set();
        });

        server.listen(TEST_PORT, "127.0.0.1");

        resetEvent.Wait(2000);
        Assert.True(listeningEmitted);
        Assert.True(server.listening);

        server.close();
    }

    [Fact]
    public void Server_Listen_WithCallback_CallsCallback()
    {
        var server = new Server();
        var resetEvent = new ManualResetEventSlim(false);
        var callbackCalled = false;

        server.listen(TEST_PORT + 1, "127.0.0.1", () =>
        {
            callbackCalled = true;
            resetEvent.Set();
        });

        resetEvent.Wait(2000);
        Assert.True(callbackCalled);

        server.close();
    }

    [Fact]
    public void Server_Close_StopsServer()
    {
        var server = new Server();
        var listenResetEvent = new ManualResetEventSlim(false);
        var closeResetEvent = new ManualResetEventSlim(false);
        var closeEmitted = false;

        server.on("listening", () => listenResetEvent.Set());
        server.on("close", () =>
        {
            closeEmitted = true;
            closeResetEvent.Set();
        });

        server.listen(TEST_PORT + 2, "127.0.0.1");
        listenResetEvent.Wait(2000);

        server.close();
        closeResetEvent.Wait(2000);

        Assert.True(closeEmitted);
        Assert.False(server.listening);
    }

    [Fact]
    public void Server_Close_WithCallback_CallsCallback()
    {
        var server = new Server();
        var listenResetEvent = new ManualResetEventSlim(false);
        var closeResetEvent = new ManualResetEventSlim(false);
        var callbackCalled = false;

        server.on("listening", () => listenResetEvent.Set());

        server.listen(TEST_PORT + 3, "127.0.0.1");
        listenResetEvent.Wait(2000);

        server.close((err) =>
        {
            callbackCalled = true;
            closeResetEvent.Set();
        });

        closeResetEvent.Wait(2000);
        Assert.True(callbackCalled);
    }

    [Fact]
    public void Server_Address_ReturnsAddressInfo()
    {
        var server = new Server();
        var resetEvent = new ManualResetEventSlim(false);

        server.on("listening", () => resetEvent.Set());
        server.listen(TEST_PORT + 4, "127.0.0.1");
        resetEvent.Wait(2000);

        var address = server.address();
        Assert.NotNull(address);
        Assert.IsType<AddressInfo>(address);

        var addrInfo = (AddressInfo)address;
        Assert.Equal(TEST_PORT + 4, addrInfo.port);
        Assert.NotEmpty(addrInfo.address);
        Assert.NotEmpty(addrInfo.family);

        server.close();
    }

    [Fact]
    public void Server_GetConnections_ReturnsCount()
    {
        var server = new Server();
        var resetEvent = new ManualResetEventSlim(false);
        var connectionCount = -1;

        server.on("listening", () => resetEvent.Set());
        server.listen(TEST_PORT + 5, "127.0.0.1");
        resetEvent.Wait(2000);

        server.getConnections((err, count) =>
        {
            connectionCount = count;
        });

        Thread.Sleep(100); // Give callback time to execute
        Assert.Equal(0, connectionCount);

        server.close();
    }

    [Fact]
    public void Server_Unref_ReturnsServer()
    {
        var server = new Server();
        var result = server.unref();
        Assert.Same(server, result);
    }

    [Fact]
    public void Server_Ref_ReturnsServer()
    {
        var server = new Server();
        var result = server.@ref();
        Assert.Same(server, result);
    }

    // ==================== Server-Client Integration Tests ====================

    [Fact]
    public void ServerClient_Connection_EmitsConnectionEvent()
    {
        var server = new Server();
        var serverResetEvent = new ManualResetEventSlim(false);
        var connectionResetEvent = new ManualResetEventSlim(false);
        Socket? serverSocket = null;

        server.on("listening", () => serverResetEvent.Set());
        server.on("connection", (Socket socket) =>
        {
            serverSocket = socket;
            connectionResetEvent.Set();
        });

        server.listen(TEST_PORT + 6, "127.0.0.1");
        serverResetEvent.Wait(2000);

        // Connect a client
        var client = net.connect(TEST_PORT + 6, "127.0.0.1");

        connectionResetEvent.Wait(2000);
        Assert.NotNull(serverSocket);

        client.destroy();
        server.close();
    }

    // ==================== BlockList Class Tests ====================

    [Fact]
    public void BlockList_Constructor_CreatesInstance()
    {
        var blockList = new BlockList();
        Assert.NotNull(blockList);
    }

    [Fact]
    public void BlockList_AddAddress_AddsToList()
    {
        var blockList = new BlockList();
        blockList.addAddress("192.168.1.1");

        Assert.True(blockList.check("192.168.1.1"));
    }

    [Fact]
    public void BlockList_AddAddress_WithType_AddsToList()
    {
        var blockList = new BlockList();
        blockList.addAddress("192.168.1.1", "ipv4");

        Assert.True(blockList.check("192.168.1.1", "ipv4"));
    }

    [Fact]
    public void BlockList_Check_NotBlocked_ReturnsFalse()
    {
        var blockList = new BlockList();
        blockList.addAddress("192.168.1.1");

        Assert.False(blockList.check("192.168.1.2"));
    }

    [Fact]
    public void BlockList_AddRange_BlocksRange()
    {
        var blockList = new BlockList();
        blockList.addRange("192.168.1.0", "192.168.1.255", "ipv4");

        Assert.True(blockList.check("192.168.1.100", "ipv4"));
        Assert.True(blockList.check("192.168.1.1", "ipv4"));
        Assert.True(blockList.check("192.168.1.255", "ipv4"));
        Assert.False(blockList.check("192.168.2.1", "ipv4"));
    }

    [Fact]
    public void BlockList_AddSubnet_BlocksSubnet()
    {
        var blockList = new BlockList();
        blockList.addSubnet("192.168.1.0", 24, "ipv4");

        Assert.True(blockList.check("192.168.1.100", "ipv4"));
        Assert.True(blockList.check("192.168.1.1", "ipv4"));
        Assert.False(blockList.check("192.168.2.1", "ipv4"));
    }

    [Fact]
    public void BlockList_GetRules_ReturnsAllRules()
    {
        var blockList = new BlockList();
        blockList.addAddress("192.168.1.1");
        blockList.addRange("10.0.0.0", "10.0.0.255", "ipv4");
        blockList.addSubnet("172.16.0.0", 16, "ipv4");

        var rules = blockList.getRules();
        Assert.NotEmpty(rules);
        Assert.True(rules.Length >= 3);
    }

    // ==================== SocketAddress Class Tests ====================

    [Fact]
    public void SocketAddress_Constructor_CreatesInstance()
    {
        var options = new SocketAddressInitOptions
        {
            address = "127.0.0.1",
            family = "ipv4",
            port = 8080
        };

        var socketAddress = new SocketAddress(options);
        Assert.NotNull(socketAddress);
        Assert.Equal("127.0.0.1", socketAddress.address);
        Assert.Equal("ipv4", socketAddress.family);
        Assert.Equal(8080, socketAddress.port);
    }

    [Fact]
    public void SocketAddress_Constructor_DefaultValues()
    {
        var options = new SocketAddressInitOptions();
        var socketAddress = new SocketAddress(options);

        Assert.Equal("0.0.0.0", socketAddress.address);
        Assert.Equal("ipv4", socketAddress.family);
        Assert.Equal(0, socketAddress.port);
    }

    [Fact]
    public void SocketAddress_Flowlabel_CanBeSet()
    {
        var options = new SocketAddressInitOptions
        {
            address = "::1",
            family = "ipv6",
            port = 8080,
            flowlabel = 12345
        };

        var socketAddress = new SocketAddress(options);
        Assert.Equal(12345, socketAddress.flowlabel);
    }

    // ==================== Options Classes Tests ====================

    [Fact]
    public void AddressInfo_AllProperties_CanBeSet()
    {
        var addressInfo = new AddressInfo
        {
            address = "192.168.1.1",
            family = "IPv4",
            port = 8080
        };

        Assert.Equal("192.168.1.1", addressInfo.address);
        Assert.Equal("IPv4", addressInfo.family);
        Assert.Equal(8080, addressInfo.port);
    }

    [Fact]
    public void SocketConstructorOpts_AllProperties_CanBeSet()
    {
        var opts = new SocketConstructorOpts
        {
            fd = 123,
            allowHalfOpen = true,
            readable = true,
            writable = true
        };

        Assert.Equal(123, opts.fd);
        Assert.True(opts.allowHalfOpen);
        Assert.True(opts.readable);
        Assert.True(opts.writable);
    }

    [Fact]
    public void TcpSocketConnectOpts_AllProperties_CanBeSet()
    {
        var opts = new TcpSocketConnectOpts
        {
            port = 8080,
            host = "localhost",
            localAddress = "127.0.0.1",
            localPort = 9090,
            hints = 1,
            family = 4,
            noDelay = true,
            keepAlive = true,
            keepAliveInitialDelay = 1000
        };

        Assert.Equal(8080, opts.port);
        Assert.Equal("localhost", opts.host);
        Assert.Equal("127.0.0.1", opts.localAddress);
        Assert.Equal(9090, opts.localPort);
        Assert.Equal(1, opts.hints);
        Assert.Equal(4, opts.family);
        Assert.True(opts.noDelay);
        Assert.True(opts.keepAlive);
        Assert.Equal(1000, opts.keepAliveInitialDelay);
    }

    [Fact]
    public void IpcSocketConnectOpts_Path_CanBeSet()
    {
        var opts = new IpcSocketConnectOpts
        {
            path = "/tmp/socket"
        };

        Assert.Equal("/tmp/socket", opts.path);
    }

    [Fact]
    public void ListenOptions_AllProperties_CanBeSet()
    {
        var opts = new ListenOptions
        {
            port = 8080,
            host = "localhost",
            path = "/tmp/socket",
            backlog = 511,
            ipv6Only = false
        };

        Assert.Equal(8080, opts.port);
        Assert.Equal("localhost", opts.host);
        Assert.Equal("/tmp/socket", opts.path);
        Assert.Equal(511, opts.backlog);
        Assert.False(opts.ipv6Only);
    }

    [Fact]
    public void ServerOpts_AllProperties_CanBeSet()
    {
        var opts = new ServerOpts
        {
            allowHalfOpen = true,
            pauseOnConnect = true
        };

        Assert.True(opts.allowHalfOpen);
        Assert.True(opts.pauseOnConnect);
    }
}
