using System;
using Xunit;

namespace nodejs.Tests;

public class AnyNaptrRecordTests
{
    [Fact]
    public void AnyNaptrRecord_HasCorrectType()
    {
        var record = new AnyNaptrRecord();
        Assert.Equal("NAPTR", record.type);
    }
}
