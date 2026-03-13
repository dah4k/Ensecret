// Copyright 2026 dah4k
// SPDX-License-Identifier: EPL-2.0

namespace Ensecret.Cmd;

using static Ensecret.Scriptlet;

class Program
{
    static void Main(string[] args)
    {
        Hello("World!");
        Hello(LoadEmbeddedFile("Files/greetings.txt"));
    }
}
