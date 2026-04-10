// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

module Ensecret.Scriptlet

open System.Diagnostics
open System.Linq
open System.IO
open System.IO.Compression
open System.Reflection
open System.Text
open System

let Hello (name: string) = printfn "Hello %s" name

let INDENTATION_CHARACTER = '\x20' // ASCII space character

let NEWLINE_CHARACTER = '\n'

let private extractIndentationPrefix (lines: string seq) : string =
    let lastLine = lines |> Seq.last
    let n = lastLine.Length
    let prefix = new String(INDENTATION_CHARACTER, n)

    if lastLine = prefix then prefix else ""

let private trimIndent (prefix: string, line: string) : string =
    let (|StartPrefix|_|) (s: string) =
        if s.StartsWith(prefix) then Some StartPrefix else None

    match line with
    | "" -> ""
    | StartPrefix -> line.Remove(0, prefix.Length)
    // This should be invalidArg "ERROR: trimIndent on outdented line".
    // Best effort trying to recover using String.TrimStart()...
    | _ -> line.TrimStart()

let private trimEnclosingNewlines (textBlock: string) : string =
    let (|OneNewline|_|) (s: string) =
        if s = $"{NEWLINE_CHARACTER}" then Some OneNewline else None

    let (|StartNewline|_|) (s: string) =
        if s.StartsWith(NEWLINE_CHARACTER) then
            Some StartNewline
        else
            None

    let (|EndNewline|_|) (s: string) =
        if s.EndsWith(NEWLINE_CHARACTER) then
            Some EndNewline
        else
            None

    match textBlock with
    | OneNewline -> ""
    | StartNewline & EndNewline -> textBlock.Substring(1, textBlock.Length - 2)
    | StartNewline -> textBlock.Substring(1, textBlock.Length - 1)
    | EndNewline -> textBlock.Substring(0, textBlock.Length - 1)
    | _ -> textBlock

let deindent (textBlock: string) : string =
    let textLines = textBlock.Split NEWLINE_CHARACTER
    let prefix = extractIndentationPrefix textLines

    textLines
    |> Seq.map (fun x -> trimIndent (prefix, x))
    |> String.concat $"{NEWLINE_CHARACTER}"
    |> trimEnclosingNewlines


// Modified from https://stackoverflow.com/questions/44659856/is-this-the-best-way-to-get-the-calling-assembly-in-windows-10-uwp
let GetParentAssemblies () =
    let currentAssembly = Assembly.GetExecutingAssembly()
    let st = new System.Diagnostics.StackTrace()

    st.GetFrames()
    |> Seq.map (fun x -> x.GetMethod().DeclaringType.GetTypeInfo().Assembly)
    |> Seq.where (fun x -> x <> currentAssembly)


let LoadEmbeddedFile (pathname: string) : byte[] =
    let assembly = GetParentAssemblies() |> Seq.head
    let name = assembly.GetName().Name

    use outStream = new MemoryStream()
    use stream = assembly.GetManifestResourceStream($"{name}.{pathname.Replace('/', '.')}")
    stream.CopyTo(outStream)
    outStream.ToArray()


let EncodeTextToBase64 (plainText: string) : string =
    plainText |> System.Text.Encoding.UTF8.GetBytes |> System.Convert.ToBase64String


let DecodeTextFromBase64 (encodedText: string) : string =
    encodedText
    |> System.Convert.FromBase64String
    |> System.Text.Encoding.UTF8.GetString


// Modified from https://khalidabuhakmeh.com/compress-strings-with-dotnet-and-csharp/
let Gzip (bytes: byte[]) : byte[] =
    // HACK: Workaround .NET 10 missing all Gzip blocks for empty byte array.
    // See Also https://github.com/dotnet/runtime/issues/122928 (2026)
    if bytes.Length = 0 then
        let gzipEmpty: byte[] =
            [|
                0x1Fuy; 0x8Buy; 0x08uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy;
                0x00uy; 0x03uy; 0x03uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy;
                0x00uy; 0x00uy; 0x00uy; 0x00uy
            |]
        gzipEmpty
    else
        use outStream = new MemoryStream()
        use gzStream = new GZipStream(outStream, CompressionMode.Compress)
        gzStream.Write(bytes, 0, bytes.Length)
        gzStream.Close()
        outStream.ToArray()


// Modified from https://khalidabuhakmeh.com/compress-strings-with-dotnet-and-csharp/
let Gunzip (bytes: byte[]) : byte[] =
    use inStream = new MemoryStream(bytes)
    use outStream = new MemoryStream()
    use gzStream = new GZipStream(inStream, CompressionMode.Decompress)
    gzStream.CopyTo(outStream)
    outStream.ToArray()


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
