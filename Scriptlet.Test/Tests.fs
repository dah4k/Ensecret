// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

module Tests

open Ensecret.Scriptlet
open Xunit

[<Fact>]
let ``Say Hello`` () =
    Hello "World!"
    Assert.True(true)

[<Fact>]
let ``Embedded resource file is not empty`` () =
    Assert.NotEmpty(LoadEmbeddedFile("Files/greetings.txt"))

[<Fact>]
let ``Embedded resource file is from us`` () =
    Assert.Matches("^from Scriptlet.Test", LoadEmbeddedFile("Files/greetings.txt"))

[<Fact>]
let ``UploadFile result contains HEREDOC`` () =
    Assert.Matches("HEREDOC", UploadFile("Files/greetings.txt", "/tmp/test.txt"))

[<Fact>]
let ``UploadFileWithChangeMode result contains chmod`` () =
    Assert.Matches("chmod", UploadFileWithChangeMode("Files/greetings.txt", "/tmp/test.txt", "640"))

[<Fact>]
let ``InstallSystemdService result contains daemon-reload`` () =
    Assert.Matches("daemon-reload", InstallSystemdService("Files/greetings.txt"))
