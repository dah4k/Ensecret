// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

module Ensecret.Tests.deindent

open Ensecret.Scriptlet
open Xunit

//==============================================================================
//  More specific Scriptlet.deindent test cases
//==============================================================================

[<Fact>]
let ``TC.0.001 - Empty string remains empty string`` () =
    let expected = ""
    let actual = "" |> deindent

    Assert.Equal(expected, actual)


[<Fact>]
let ``TC.0.100 - No-indent zero line block becomes empty string`` () =
    let expected = ""

    let actual =
        """
"""
        |> deindent

    Assert.Equal(expected, actual)


[<Fact>]
let ``TC.0.101 - No-indent one empty line block becomes empty string`` () =
    let expected = ""

    let actual =
        """

"""
        |> deindent

    Assert.Equal(expected, actual)


[<Fact>]
let ``TC.0.102 - No-indent two empty lines block becomes one newline`` () =
    let expected = "\n"

    let actual =
        """


"""
        |> deindent

    Assert.Equal(expected, actual)


[<Fact>]
let ``TC.0.200 - Indent-8 zero line block becomes empty string`` () =
    let expected = ""

    let actual =
        """
        """
        |> deindent

    Assert.Equal(expected, actual)


[<Fact>]
[<Trait("Category", "deindent")>]
let ``TC.0.201 - Indent-8 one empty line block becomes empty string`` () =
    let expected = ""

    let actual =
        """

        """
        |> deindent

    Assert.Equal(expected, actual)


[<Fact>]
let ``TC.0.202 - Indent-8 two empty lines becomes one newline`` () =
    let expected = "\n"

    let actual =
        """


        """
        |> deindent

    Assert.Equal(expected, actual)
