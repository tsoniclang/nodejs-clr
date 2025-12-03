using System;
using Xunit;

namespace nodejs.Tests;

public class AnyMxRecordTests
{
    [Fact]
    public void AnyMxRecord_HasCorrectType()
    {
        var record = new AnyMxRecord();
        Assert.Equal("MX", record.type);
    }
}
