// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

namespace Ensecret

open System.Diagnostics
open System.Linq
open System.IO
open System.Reflection
open System.Text
open System

module Scriptlet =

    let Hello (name: string) = printfn "Hello %s" name

    let INDENTATION_CHARACTER = '\x20' // ASCII space character

    let extractIndentationPrefix (line: string) : string =
        let n = line.Length
        let prefix = new String(INDENTATION_CHARACTER, n)

        if line = prefix then prefix else String.Empty

    let trimIndent (prefix: string, line: string) : string =
        if prefix = String.Empty then
            line
        elif line = String.Empty then
            line
        elif line.StartsWith(prefix) then
            line.Remove(0, prefix.Length)
        else
            // This should be invalidArg "ERROR: trimIndent on outdented line".
            // Workaround to recover using String.TrimStart() for now...
            line.TrimStart()

    let trimEnclosingNewlines (textBlock: string) : string = textBlock

    let deindent (textBlock: string) : string =
        let textLines = textBlock.Split '\n'
        let prefix = textLines |> Seq.last |> extractIndentationPrefix

        textLines
        |> Seq.map (fun x -> trimIndent (prefix, x))
        |> Seq.reduce (fun x y -> x + "\n" + y)
        |> trimEnclosingNewlines


    // Modified from https://stackoverflow.com/questions/44659856/is-this-the-best-way-to-get-the-calling-assembly-in-windows-10-uwp
    let GetParentAssemblies () =
        let currentAssembly = Assembly.GetExecutingAssembly()
        let st = new System.Diagnostics.StackTrace()

        st.GetFrames()
        |> Seq.map (fun x -> x.GetMethod().DeclaringType.GetTypeInfo().Assembly)
        |> Seq.where (fun x -> x <> currentAssembly)


    let LoadEmbeddedFile (pathname: string) : string =
        let assembly = GetParentAssemblies() |> Seq.head
        let name = assembly.GetName().Name

        use stream =
            assembly.GetManifestResourceStream($"{name}.{pathname.Replace('/', '.')}")

        use streamReader = new StreamReader(stream, Encoding.UTF8)
        streamReader.ReadToEnd()


    let UploadFile (localPath: string, remotePath: string) : string =
        $$"""
        sudo mkdir --parents ${dirname {{remotePath}}}

        cat <<'END_OF_HEREDOC_UPLOAD_FILE' | sudo tee {{remotePath}}
        {{LoadEmbeddedFile(localPath)}}
        END_OF_HEREDOC_UPLOAD_FILE
        """
        |> deindent


    let UploadFileWithChangeMode (localPath: string, remotePath: string, mode: string) : string =
        $$"""
        {{UploadFile(localPath, remotePath)}}

        sudo chmod {{mode}} {{remotePath}}
        """
        |> deindent


    let InstallSystemdService (localServicePath: string) : string =
        let serviceName = Path.GetFileName(localServicePath)
        let remoteServicePath = $"/etc/systemd/system/{serviceName}"

        $$"""
        {{UploadFile(localServicePath, remoteServicePath)}}

        sudo systemctl daemon-reload
        sudo systemctl enable {{serviceName}}
        sudo systemctl start {{serviceName}}
        """
        |> deindent
