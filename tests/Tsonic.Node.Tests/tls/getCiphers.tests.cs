using System;
using Xunit;

namespace Tsonic.Node.Tests;

public class Tls_getCiphersTests
{
    [Fact]
    public void getCiphers_ReturnsArray()
    {
        var ciphers = tls.getCiphers();
        Assert.NotNull(ciphers);
        Assert.NotEmpty(ciphers);
    }
}
