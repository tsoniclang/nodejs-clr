using Xunit;
using System;

namespace nodejs.Tests;

public class setDefaultEncodingTests
{
    [Fact]
    public void setDefaultEncoding_DoesNotThrow()
    {
        crypto.setDefaultEncoding("hex");
        crypto.setDefaultEncoding("base64");
    }
}
