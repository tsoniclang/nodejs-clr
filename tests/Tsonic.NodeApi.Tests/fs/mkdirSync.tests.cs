using Xunit;

namespace Tsonic.NodeApi.Tests;

public class mkdirSyncTests : FsTestBase
{
    [Fact]
    public void mkdirSync_ShouldCreateDirectory()
    {
        var dirPath = GetTestPath("new-dir");

        fs.mkdirSync(dirPath);

        Assert.True(Directory.Exists(dirPath));
    }

    [Fact]
    public void mkdirSync_Recursive_ShouldCreateNestedDirectories()
    {
        var dirPath = GetTestPath("parent/child/grandchild");

        fs.mkdirSync(dirPath, recursive: true);

        Assert.True(Directory.Exists(dirPath));
    }

    [Fact]
    public void mkdirSync_NonRecursive_MissingParent_ShouldThrow()
    {
        var dirPath = GetTestPath("missing-parent/child");

        Assert.Throws<DirectoryNotFoundException>(() => fs.mkdirSync(dirPath, recursive: false));
    }
}
