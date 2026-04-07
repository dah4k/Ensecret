// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

namespace Csharp.GzipStream;

using System.IO.Compression;
using System.Text;

public class Tests
{
    // Modified from https://khalidabuhakmeh.com/compress-strings-with-dotnet-and-csharp/
    byte[] GzipText(string plainText)
    {
        using (var inStream = new MemoryStream(Encoding.UTF8.GetBytes(plainText)))
        using (var outStream = new MemoryStream())
        using (var gzStream = new GZipStream(outStream, CompressionMode.Compress))
        {
            inStream.CopyTo(gzStream);
            gzStream.Close();
            return outStream.ToArray();
        }
    }

    // Modified from https://github.com/dotnet/runtime/issues/122928 (Not planned 2026)
    // See Also
    // - https://github.com/dotnet/runtime/issues/112563 (Closed 2025)
    // - https://github.com/dotnet/runtime/pull/94433 (Merged 2023)
    byte[] Gzip(byte[] data)
    {
        using var outStream = new MemoryStream();
        using (var gzStream = new GZipStream(outStream, CompressionMode.Compress, true))
        {
            gzStream.Write(data, 0, data.Length);
        }
        return outStream.ToArray();
    }

    [Fact]
    public void GzipTextAndBase64_abcd_string()
    {
        // echo -n "abcd" | gzip | base64
        string expected = "H4sIAAAAAAAAA0tMSk4BABHNgu0EAAAA";
        string actual = System.Convert.ToBase64String(GzipText("abcd"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GzipTextAndBase64_empty_string()
    {
        // echo -n "" | gzip | base64
        string expected = "H4sIAAAAAAAAAwMAAAAAAAAAAAA=";
        string actual = System.Convert.ToBase64String(GzipText(""));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Gzip_non_empty_array()
    {
        byte[] nonEmptyArray = Encoding.UTF8.GetBytes("abcd");
        Assert.NotEmpty(Gzip(nonEmptyArray));
    }

    [Fact]
    public void Gzip_empty_array()
    {
        byte[] emptyArray = Array.Empty<byte>();
        Assert.NotEmpty(Gzip(emptyArray));
    }
}
