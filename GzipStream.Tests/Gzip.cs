// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

namespace Csharp.GzipStream;

using System.IO.Compression;
using System.Text;

public class Tests
{
    byte[] Gzip(byte[] data)
    {
        // HACK: Workaround .NET 10 missing all Gzip blocks for empty byte array.
        // See Also https://github.com/dotnet/runtime/issues/122928
        if (0 == data.Length) {
            // echo -n "" | gzip | xxd -i
            return new byte[] {
                0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };
        }

        using (var outStream = new MemoryStream())
        using (var gzStream = new GZipStream(outStream, CompressionMode.Compress))
        {
            gzStream.Write(data, 0, data.Length);
            gzStream.Close();
            return outStream.ToArray();
        }
    }

    [Fact]
    public void GzipAndBase64_abcd_string()
    {
        // echo -n "abcd" | gzip | base64
        string expected = "H4sIAAAAAAAAA0tMSk4BABHNgu0EAAAA";
        string actual = System.Convert.ToBase64String(Gzip(Encoding.UTF8.GetBytes("abcd")));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GzipAndBase64_empty_string()
    {
        // echo -n "" | gzip | base64
        string expected = "H4sIAAAAAAAAAwMAAAAAAAAAAAA=";
        string actual = System.Convert.ToBase64String(Gzip(Encoding.UTF8.GetBytes("")));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Gzip_non_empty_array()
    {
        byte[] nonEmptyArray = {0x00};
        Assert.NotEmpty(Gzip(nonEmptyArray));
    }

    [Fact]
    public void Gzip_empty_array()
    {
        byte[] emptyArray = Array.Empty<byte>();
        Assert.NotEmpty(Gzip(emptyArray));
    }
}
