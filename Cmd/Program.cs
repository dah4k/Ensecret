// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

namespace Ensecret.Cmd;

using static Ensecret.Scriptlet;

class Program
{
    static void Main(string[] args)
    {
        Hello("World!");
        Hello(System.Text.Encoding.UTF8.GetString(LoadEmbeddedFile("Files/greetings.txt")));
        Hello(UploadFile("Files/greetings.txt", "/tmp/test.txt"));
        Hello(UploadFileWithChangeMode("Files/greetings.txt", "/tmp/test.txt", "640"));
        Hello(InstallSystemdService("Files/greetings.txt"));
    }
}
