using Xunit;
using System;

namespace nodejs.Tests;

public class setFipsTests
{
    [Fact]
    public void setFips_False_DoesNotThrow()
    {
        crypto.setFips(false);
    }

    [Fact]
    public void setFips_True_Throws()
    {
        Assert.Throws<NotSupportedException>(() => crypto.setFips(true));
    }
}
