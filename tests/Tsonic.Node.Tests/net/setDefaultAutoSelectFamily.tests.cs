using System;
using Xunit;

namespace Tsonic.Node.Tests;

public class setDefaultAutoSelectFamilyTests
{
    [Fact]
    public void setDefaultAutoSelectFamily_UpdatesValue()
    {
        var original = net.getDefaultAutoSelectFamily();
        net.setDefaultAutoSelectFamily(!original);
        Assert.Equal(!original, net.getDefaultAutoSelectFamily());
        // Reset to original
        net.setDefaultAutoSelectFamily(original);
    }
}
