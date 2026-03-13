// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

module Tests

open Ensecret.Scriptlet
open Xunit

[<Fact>]
let ``Say Hello`` () =
    Hello "World!"
    Assert.True(true)
