using System;
using Xunit;

namespace nodejs.Tests;

public class AnySoaRecordTests
{
    [Fact]
    public void AnySoaRecord_HasCorrectType()
    {
        var record = new AnySoaRecord();
        Assert.Equal("SOA", record.type);
    }
}
