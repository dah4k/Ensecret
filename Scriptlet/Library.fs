// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

namespace Ensecret

open System.Diagnostics
open System.Linq
open System.IO
open System.Reflection
open System.Text

module Scriptlet =

    let Hello (name: string) = printfn "Hello %s" name

    let deindent (textBlock: string) : string = textBlock

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
