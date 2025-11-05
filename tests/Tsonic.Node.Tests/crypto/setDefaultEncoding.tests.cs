using Xunit;
using System;

namespace Tsonic.Node.Tests;

public class setDefaultEncodingTests
{
    [Fact]
    public void setDefaultEncoding_DoesNotThrow()
    {
        crypto.setDefaultEncoding("hex");
        crypto.setDefaultEncoding("base64");
    }
}
