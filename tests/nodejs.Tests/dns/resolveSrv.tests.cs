using System;
using System.Threading;
using Xunit;

namespace nodejs.Tests;

public class resolveSrvTests
{
    [Fact]
    public void resolveSrv_ValidDomain_ReturnsSrvRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        SrvRecord[]? records = null;

        dns.resolveSrv("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }
}
