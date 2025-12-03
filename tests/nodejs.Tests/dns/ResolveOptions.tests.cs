using System;
using Xunit;

namespace nodejs.Tests;

public class ResolveOptionsTests
{
    [Fact]
    public void ResolveOptions_TtlProperty_CanBeSet()
    {
        var options = new ResolveOptions { ttl = true };
        Assert.True(options.ttl);
    }
}
