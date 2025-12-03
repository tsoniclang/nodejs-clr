using System;
using Xunit;

namespace nodejs.Tests;

public class ServerOptsTests
{
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
