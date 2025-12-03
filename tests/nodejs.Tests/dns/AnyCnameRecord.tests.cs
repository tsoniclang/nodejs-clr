using System;
using Xunit;

namespace nodejs.Tests;

public class AnyCnameRecordTests
{
    [Fact]
    public void AnyCnameRecord_AllProperties_CanBeSet()
    {
        var record = new AnyCnameRecord
        {
            value = "www.example.com"
        };

        Assert.Equal("CNAME", record.type);
        Assert.Equal("www.example.com", record.value);
    }
}
