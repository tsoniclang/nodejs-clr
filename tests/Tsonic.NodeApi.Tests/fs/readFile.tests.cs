using Xunit;

namespace Tsonic.NodeApi.Tests;

public class readFileTests : FsTestBase
{
    [Fact]
    public async Task readFile_ShouldReadTextFile()
    {
        var filePath = GetTestPath("test-async.txt");
        var content = "Hello, Async World!";
        File.WriteAllText(filePath, content);

        var result = await fs.readFile(filePath, "utf-8");
        Assert.Equal(content, result);
    }

    [Fact]
    public async Task readFile_NonExistentFile_ShouldThrow()
    {
        var filePath = GetTestPath("nonexistent-async.txt");
        await Assert.ThrowsAsync<FileNotFoundException>(async () => await fs.readFile(filePath, "utf-8"));
    }
}
