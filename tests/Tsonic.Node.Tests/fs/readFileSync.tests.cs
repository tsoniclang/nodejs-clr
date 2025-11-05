using Xunit;

namespace Tsonic.Node.Tests;

public class readFileSyncTests : FsTestBase
{
    [Fact]
    public void readFileSync_ShouldReadTextFile()
    {
        var filePath = GetTestPath("test.txt");
        var content = "Hello, World!";
        File.WriteAllText(filePath, content);

        var result = fs.readFileSync(filePath, "utf-8");
        Assert.Equal(content, result);
    }

    [Fact]
    public void readFileSync_NonExistentFile_ShouldThrow()
    {
        var filePath = GetTestPath("nonexistent.txt");
        Assert.Throws<FileNotFoundException>(() => fs.readFileSync(filePath, "utf-8"));
    }
}
