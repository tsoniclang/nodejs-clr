using Xunit;
using System;

namespace Tsonic.Node.Tests;

public class getDefaultCipherListTests
{
    [Fact]
    public void getDefaultCipherList_ReturnsString()
    {
        var list = crypto.getDefaultCipherList();
        Assert.NotEmpty(list);
        Assert.Contains(":", list);
    }
}
