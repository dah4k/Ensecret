// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

module Tests.API

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
