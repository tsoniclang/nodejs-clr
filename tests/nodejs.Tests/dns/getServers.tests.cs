using System;
using Xunit;

namespace nodejs.Tests;

public class getServersTests
{
    [Fact]
    public void getServers_ReturnsServerArray()
    {
        var servers = dns.getServers();
        Assert.NotNull(servers);
        Assert.IsType<string[]>(servers);
    }
}
