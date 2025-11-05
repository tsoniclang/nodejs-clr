using System;
using System.Text;

namespace Tsonic.Node;

/// <summary>
/// Provides an API for decoding Buffer objects into strings in a manner that preserves encoded multi-byte UTF-8 and UTF-16 characters.
/// </summary>
public class StringDecoder
{
    private readonly Encoding _encoding;
    private readonly Decoder _decoder;

    /// <summary>
    /// Creates a new StringDecoder instance.
    /// </summary>
    /// <param name="encoding">The character encoding to use. Default is 'utf8'.</param>
    public StringDecoder(string? encoding = null)
    {
        encoding ??= "utf8";

        _encoding = encoding.ToLowerInvariant() switch
        {
            "utf8" or "utf-8" => Encoding.UTF8,
            "utf16le" or "utf-16le" => Encoding.Unicode,
            "utf16be" or "utf-16be" => Encoding.BigEndianUnicode,
            "ascii" => Encoding.ASCII,
            "latin1" or "binary" => Encoding.Latin1,
            _ => Encoding.UTF8
        };

        _decoder = _encoding.GetDecoder();
    }

    /// <summary>
    /// Returns a decoded string, ensuring that any incomplete multibyte characters at the end of the Buffer are omitted from the returned string and stored in an internal buffer for the next call.
    /// </summary>
    /// <param name="buffer">The bytes to decode.</param>
    /// <returns>The decoded string.</returns>
    public string write(byte[] buffer)
    {
        if (buffer == null || buffer.Length == 0)
            return string.Empty;

        // Use the decoder with flush: false to preserve incomplete sequences
        int charCount = _decoder.GetCharCount(buffer, 0, buffer.Length, false);
        char[] chars = new char[charCount];
        _decoder.GetChars(buffer, 0, buffer.Length, chars, 0, false);

        return new string(chars);
    }

    /// <summary>
    /// Returns any remaining input stored in the internal buffer as a string. After end() is called, the StringDecoder object can be reused for new input.
    /// </summary>
    /// <param name="buffer">Optional final bytes to decode.</param>
    /// <returns>The decoded string.</returns>
    public string end(byte[]? buffer = null)
    {
        string result = "";

        if (buffer != null && buffer.Length > 0)
        {
            // Decode with flush: true to output any remaining bytes
            int charCount = _decoder.GetCharCount(buffer, 0, buffer.Length, true);
            char[] chars = new char[charCount];
            _decoder.GetChars(buffer, 0, buffer.Length, chars, 0, true);
            result = new string(chars);
        }
        else
        {
            // Flush any remaining incomplete bytes
            int charCount = _decoder.GetCharCount(Array.Empty<byte>(), 0, 0, true);
            if (charCount > 0)
            {
                char[] chars = new char[charCount];
                _decoder.GetChars(Array.Empty<byte>(), 0, 0, chars, 0, true);
                result = new string(chars);
            }
        }

        // Reset the decoder for potential reuse
        _decoder.Reset();

        return result;
    }
}
