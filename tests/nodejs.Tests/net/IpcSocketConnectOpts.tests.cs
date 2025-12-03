using System;
using Xunit;

namespace nodejs.Tests;

public class IpcSocketConnectOptsTests
{
    [Fact]
    public void IpcSocketConnectOpts_Path_CanBeSet()
    {
        var opts = new IpcSocketConnectOpts
        {
            path = "/tmp/socket"
        };

        Assert.Equal("/tmp/socket", opts.path);
    }
}
