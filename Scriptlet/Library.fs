// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

namespace Ensecret

open System.IO
open System.Reflection
open System.Text

module Scriptlet =

    let Hello (name: string) = printfn "Hello %s" name

    let LoadEmbeddedFile (pathname: string) : string =
        let assembly = Assembly.GetCallingAssembly()
        let name = assembly.GetName().Name

        use stream =
            assembly.GetManifestResourceStream($"{name}.{pathname.Replace('/', '.')}")

        use streamReader = new StreamReader(stream, Encoding.UTF8)
        streamReader.ReadToEnd()
