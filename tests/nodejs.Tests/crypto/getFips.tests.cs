using Xunit;
using System;

namespace nodejs.Tests;

public class getFipsTests
{
    [Fact]
    public void getFips_ReturnsFalse()
    {
        // In .NET, FIPS mode is not directly configurable
        Assert.False(crypto.getFips());
    }
}
