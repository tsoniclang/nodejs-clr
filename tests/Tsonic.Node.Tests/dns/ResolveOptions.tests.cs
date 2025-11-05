using System;
using Xunit;

namespace Tsonic.Node.Tests;

public class ResolveOptionsTests
{
    [Fact]
    public void ResolveOptions_TtlProperty_CanBeSet()
    {
        var options = new ResolveOptions { ttl = true };
        Assert.True(options.ttl);
    }
}
