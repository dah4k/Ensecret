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


    let UploadFile (localPath: string, remotePath: string) : string =
        $$"""
        sudo mkdir --parents ${dirname {{remotePath}}}

        cat <<'END_OF_HEREDOC_UPLOAD_FILE' | sudo tee {{remotePath}}
        {{LoadEmbeddedFile(localPath)}}
        END_OF_HEREDOC_UPLOAD_FILE
        """


    let UploadFileWithChangeMode (localPath: string, remotePath: string, mode: string) : string =
        $$"""
        {{UploadFile(localPath, remotePath)}}

        sudo chmod {{mode}} {{remotePath}}
        """


    let InstallSystemdService (localServicePath: string) : string =
        let serviceName = Path.GetFileName(localServicePath)
        let remoteServicePath = $"/etc/systemd/system/{serviceName}"

        $$"""
        {{UploadFile(localServicePath, remoteServicePath)}}

        sudo systemctl daemon-reload
        sudo systemctl enable {{serviceName}}
        sudo systemctl start {{serviceName}}
        """
