// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

namespace GzipStream.Tests;

using System.IO.Compression;
using System.Text;

public class UnitTest1
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

    [Fact]
    public void GzipBase64_abcd_string()
    {
        // echo -n "abcd" | gzip | base64
        string expected = "H4sIAAAAAAAAA0tMSk4BABHNgu0EAAAA";
        string actual = System.Convert.ToBase64String(GzipText("abcd"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GzipBase64_empty_string()
    {
        // echo -n "" | gzip | base64
        string expected = "H4sIAAAAAAAAAwMAAAAAAAAAAAA=";
        string actual = System.Convert.ToBase64String(GzipText(""));

        Assert.Equal(expected, actual);
    }
}
