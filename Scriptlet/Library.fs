// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

module Ensecret.Scriptlet

open System.IO
open System.Reflection
open System.Text

let Hello (name: string) =
    printfn "Hello %s" name

let LoadEmbeddedFile (pathname: string) : string =
    let assembly = Assembly.GetExecutingAssembly()
    let name = assembly.GetName().Name
    use stream = assembly.GetManifestResourceStream($"{name}.{pathname.Replace('/', '.')}")
    use streamReader = new StreamReader(stream, Encoding.UTF8)
    streamReader.ReadToEnd()

