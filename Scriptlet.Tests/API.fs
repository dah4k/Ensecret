// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

module Ensecret.Tests.API

open Ensecret.Scriptlet
open Xunit

//==============================================================================
//  Scriptlet public API test cases
//==============================================================================

[<Fact>]
let ``TC.1.000 - Say Hello`` () =
    Hello "World!"
    Assert.True(true)

[<Fact>]
let ``TC.1.100 - Embedded resource file is not empty`` () =
    Assert.NotEmpty(LoadEmbeddedFile("Files/greetings.txt"))

[<Fact>]
let ``TC.1.110 - Embedded resource file is from us`` () =
    Assert.Matches("^from Scriptlet.Tests", LoadEmbeddedFile("Files/greetings.txt"))

[<Fact>]
let ``TC.1.200 - UploadFile result contains HEREDOC`` () =
    Assert.Matches("HEREDOC", UploadFile("Files/greetings.txt", "/tmp/test.txt"))

[<Fact>]
let ``TC.1.210 - UploadFileWithChangeMode result contains HEREDOC`` () =
    Assert.Matches("HEREDOC", UploadFileWithChangeMode("Files/greetings.txt", "/tmp/test.txt", "640"))

[<Fact>]
let ``TC.1.211 - UploadFileWithChangeMode result contains chmod`` () =
    Assert.Matches("chmod", UploadFileWithChangeMode("Files/greetings.txt", "/tmp/test.txt", "640"))

[<Fact>]
let ``TC.1.300 - InstallSystemdService result contains daemon-reload`` () =
    Assert.Matches("daemon-reload", InstallSystemdService("Files/greetings.txt"))

[<Fact>]
// Reference: https://github.com/dotnet/fsharp/issues/13995
let ``TC.1.400 - Indentation aware raw string literals (similar to C# 11) #13995`` () =
    let htmlString1 =
        """<!DOCTYPE html>
<html lang="en">
    <head>
    </head>
    <body>
        <h1>Hello</h1>
     </body>
</html>"""

    let htmlString2 =
        """
        <!DOCTYPE html>
        <html lang="en">
            <head>
            </head>
            <body>
                <h1>Hello</h1>
             </body>
        </html>
        """
        |> deindent

    Assert.Equal(htmlString1, htmlString2)

[<Fact>]
let ``TC.1.500 - EncodeTextToBase64 empty string returns empty string`` () =
    let expected = ""
    let actual = "" |> EncodeTextToBase64

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.501 - EncodeTextToBase64 "abcd" returns "YWJjZA=="`` () =
    let expected = "YWJjZA=="
    let actual = "abcd" |> EncodeTextToBase64

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.510 - DecodeTextFromBase64 empty string returns empty string`` () =
    let expected = ""
    let actual = "" |> DecodeTextFromBase64

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.511 - DecodeTextFromBase64 "YWJjZA==" returns "abcd"`` () =
    let expected = "abcd"
    let actual = "YWJjZA==" |> DecodeTextFromBase64

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.520 - "" |> GzipText |> ToBase64String`` () =
    let expected = "H4sIAAAAAAAAAwMAAAAAAAAAAAA="
    let actual = "" |> GzipText |> System.Convert.ToBase64String

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.521 - "abcd" |> GzipText |> ToBase64String`` () =
    let expected = "H4sIAAAAAAAAA0tMSk4BABHNgu0EAAAA"
    let actual = "abcd" |> GzipText |> System.Convert.ToBase64String

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.530 - "H4sIAAAAAAAAAwMAAAAAAAAAAAA=" |> FromBase64String |> GunzipBytes`` () =
    let expected = ""

    let actual =
        "H4sIAAAAAAAAAwMAAAAAAAAAAAA=" |> System.Convert.FromBase64String |> GunzipBytes

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.531 - "H4sIAAAAAAAAA0tMSk4BABHNgu0EAAAA" |> FromBase64String |> GunzipBytes`` () =
    let expected = "abcd"

    let actual =
        "H4sIAAAAAAAAA0tMSk4BABHNgu0EAAAA"
        |> System.Convert.FromBase64String
        |> GunzipBytes

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.540 - "" |> GzipText |> GunzipBytes`` () =
    let expected = ""
    let actual = expected |> GzipText |> GunzipBytes

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.541 - "abcd" |> GzipText |> GunzipBytes`` () =
    let expected = "abcd"
    let actual = expected |> GzipText |> GunzipBytes

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.550 - "H4sIAAAAAAAAAwMAAAAAAAAAAAA=" |> FromBase64String |> GunzipBytes |> GzipText |> ToBase64String`` () =
    let expected = "H4sIAAAAAAAAAwMAAAAAAAAAAAA="

    let actual =
        expected
        |> System.Convert.FromBase64String
        |> GunzipBytes
        |> GzipText
        |> System.Convert.ToBase64String

    Assert.Equal(expected, actual)

[<Fact>]
let ``TC.1.551 - echo -n "H4sIAAAAAAAAA0tMSk4BABHNgu0EAAAA" | base64 -d | gzip | gunzip | base64`` () =
    let expected = "H4sIAAAAAAAAA0tMSk4BABHNgu0EAAAA"

    let actual =
        expected
        |> System.Convert.FromBase64String
        |> GunzipBytes
        |> GzipText
        |> System.Convert.ToBase64String

    Assert.Equal(expected, actual)
