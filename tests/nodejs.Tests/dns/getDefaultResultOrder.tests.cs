using System;
using Xunit;

namespace nodejs.Tests;

public class getDefaultResultOrderTests
{
    [Fact]
    public void getDefaultResultOrder_ReturnsVerbatim()
    {
        var order = dns.getDefaultResultOrder();
        Assert.Equal("verbatim", order);
    }
}
