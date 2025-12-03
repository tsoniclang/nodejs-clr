using System;
using Xunit;

namespace nodejs.Tests;

public class getCACertificatesTests
{
    [Fact]
    public void getCACertificates_ReturnsArray()
    {
        var certs = tls.getCACertificates();
        Assert.NotNull(certs);
        // May be empty depending on system
    }
}
