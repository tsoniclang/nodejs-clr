using System;
using Xunit;

namespace nodejs.Tests;

public class addressTests
{
    [Fact]
    public void address_UnboundSocket_ThrowsException()
    {
        var socket = dgram.createSocket("udp4");
        Assert.Throws<InvalidOperationException>(() => socket.address());
        socket.close();
    }
}
