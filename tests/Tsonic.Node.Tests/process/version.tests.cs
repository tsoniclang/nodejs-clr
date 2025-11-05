using Xunit;

namespace Tsonic.Node.Tests;

public class versionTests
{
    [Fact]
    public void version_ShouldReturnValidVersionString()
    {
        var version = process.version;

        Assert.NotNull(version);
        Assert.NotEmpty(version);
        Assert.StartsWith("v", version);
    }

    [Fact]
    public void version_ShouldContainTsonicIdentifier()
    {
        var version = process.version;

        Assert.Contains("tsonic", version.ToLowerInvariant());
    }
}
