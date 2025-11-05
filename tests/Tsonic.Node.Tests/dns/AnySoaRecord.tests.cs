using System;
using Xunit;

namespace Tsonic.Node.Tests;

public class AnySoaRecordTests
{
    [Fact]
    public void AnySoaRecord_HasCorrectType()
    {
        var record = new AnySoaRecord();
        Assert.Equal("SOA", record.type);
    }
}
