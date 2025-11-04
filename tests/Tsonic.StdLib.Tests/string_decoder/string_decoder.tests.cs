using System.Text;
using Xunit;

namespace Tsonic.StdLib.Tests;

public class StringDecoderTests
{
    [Fact]
    public void write_ShouldDecodeSimpleUtf8String()
    {
        var decoder = new StringDecoder("utf8");
        var bytes = Encoding.UTF8.GetBytes("hello world");

        var result = decoder.write(bytes);

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void write_ShouldHandleEmptyBuffer()
    {
        var decoder = new StringDecoder("utf8");
        var bytes = new byte[0];

        var result = decoder.write(bytes);

        Assert.Equal("", result);
    }

    [Fact]
    public void write_ShouldPreserveIncompleteMultibyteSequence()
    {
        var decoder = new StringDecoder("utf8");

        // Euro symbol (€) is 3 bytes in UTF-8: E2 82 AC
        var bytes1 = new byte[] { 0xE2 };
        var bytes2 = new byte[] { 0x82 };
        var bytes3 = new byte[] { 0xAC };

        var result1 = decoder.write(bytes1);
        Assert.Equal("", result1); // Incomplete sequence

        var result2 = decoder.write(bytes2);
        Assert.Equal("", result2); // Still incomplete

        var result3 = decoder.write(bytes3);
        Assert.Equal("€", result3); // Complete sequence
    }

    [Fact]
    public void write_ShouldHandleMultipleCompleteCharacters()
    {
        var decoder = new StringDecoder("utf8");

        // Test with multiple multi-byte characters
        var bytes = Encoding.UTF8.GetBytes("Hello 世界");

        var result = decoder.write(bytes);

        Assert.Equal("Hello 世界", result);
    }

    [Fact]
    public void write_ShouldHandleAsciiEncoding()
    {
        var decoder = new StringDecoder("ascii");
        var bytes = Encoding.ASCII.GetBytes("hello");

        var result = decoder.write(bytes);

        Assert.Equal("hello", result);
    }

    [Fact]
    public void write_ShouldHandleLatin1Encoding()
    {
        var decoder = new StringDecoder("latin1");
        var bytes = Encoding.Latin1.GetBytes("café");

        var result = decoder.write(bytes);

        Assert.Equal("café", result);
    }

    [Fact]
    public void write_ShouldDefaultToUtf8()
    {
        var decoder = new StringDecoder();
        var bytes = Encoding.UTF8.GetBytes("hello");

        var result = decoder.write(bytes);

        Assert.Equal("hello", result);
    }

    [Fact]
    public void end_ShouldReturnEmptyStringWithoutBuffer()
    {
        var decoder = new StringDecoder("utf8");

        var result = decoder.end();

        Assert.Equal("", result);
    }

    [Fact]
    public void end_ShouldDecodeOptionalBuffer()
    {
        var decoder = new StringDecoder("utf8");
        var bytes = Encoding.UTF8.GetBytes("hello");

        var result = decoder.end(bytes);

        Assert.Equal("hello", result);
    }

    [Fact]
    public void end_ShouldFlushIncompleteBytes()
    {
        var decoder = new StringDecoder("utf8");

        // Start of a multi-byte sequence
        var bytes = new byte[] { 0xE2, 0x82 };

        decoder.write(bytes);
        var result = decoder.end();

        // Should return something (substitution character or the incomplete bytes)
        Assert.NotNull(result);
    }

    [Fact]
    public void end_ShouldAllowReuse()
    {
        var decoder = new StringDecoder("utf8");

        // First use
        var bytes1 = Encoding.UTF8.GetBytes("hello");
        var result1 = decoder.end(bytes1);
        Assert.Equal("hello", result1);

        // Second use after end()
        var bytes2 = Encoding.UTF8.GetBytes("world");
        var result2 = decoder.write(bytes2);
        Assert.Equal("world", result2);
    }

    [Fact]
    public void constructor_ShouldAcceptNull()
    {
        var decoder = new StringDecoder(null);

        var bytes = Encoding.UTF8.GetBytes("hello");
        var result = decoder.write(bytes);

        Assert.Equal("hello", result);
    }

    [Fact]
    public void multipleWrites_ShouldAccumulateIncompleteBytes()
    {
        var decoder = new StringDecoder("utf8");

        // Split "hello世界" across multiple writes
        var fullBytes = Encoding.UTF8.GetBytes("hello世界");

        // Write first part
        var part1 = new byte[7]; // "hello" + part of first Chinese character
        Array.Copy(fullBytes, 0, part1, 0, 7);
        var result1 = decoder.write(part1);

        // Write remaining part
        var part2 = new byte[fullBytes.Length - 7];
        Array.Copy(fullBytes, 7, part2, 0, part2.Length);
        var result2 = decoder.write(part2);

        // Combined should give us the full string
        var combined = result1 + result2;
        Assert.Equal("hello世界", combined);
    }

    [Fact]
    public void utf16_ShouldDecodeCorrectly()
    {
        var decoder = new StringDecoder("utf16le");
        var bytes = Encoding.Unicode.GetBytes("hello");

        var result = decoder.write(bytes);

        Assert.Equal("hello", result);
    }

    [Fact]
    public void write_ShouldHandleNullBuffer()
    {
        var decoder = new StringDecoder("utf8");

        var result = decoder.write(null!);

        Assert.Equal("", result);
    }

    [Fact]
    public void partialMultibyte_InMiddleOfBuffer()
    {
        var decoder = new StringDecoder("utf8");

        // "a" + incomplete Euro symbol
        var bytes = new byte[] { 0x61, 0xE2, 0x82 };

        var result1 = decoder.write(bytes);
        Assert.Equal("a", result1); // 'a' is complete, Euro is incomplete

        // Complete the Euro symbol
        var bytes2 = new byte[] { 0xAC };
        var result2 = decoder.write(bytes2);
        Assert.Equal("€", result2);
    }

    [Fact]
    public void complexScenario_MixedEncodingsOverMultipleWrites()
    {
        var decoder = new StringDecoder("utf8");

        // Test a complex real-world scenario
        var text = "Hello 🌍 World! Café ☕";
        var allBytes = Encoding.UTF8.GetBytes(text);

        var result = "";

        // Split into random chunks and decode
        int offset = 0;
        int[] chunkSizes = { 5, 3, 7, 4, allBytes.Length - 19 };

        foreach (var size in chunkSizes)
        {
            if (offset >= allBytes.Length) break;

            var actualSize = Math.Min(size, allBytes.Length - offset);
            var chunk = new byte[actualSize];
            Array.Copy(allBytes, offset, chunk, 0, actualSize);

            result += decoder.write(chunk);
            offset += actualSize;
        }

        result += decoder.end();

        Assert.Equal(text, result);
    }
}
