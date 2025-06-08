using System;
using AlkaOS.Kernel.FileSystem;

namespace AlkaOS.Kernel.FileSystem;

public class FileSystemConsole {
    private readonly SimpleFileSystem fs = new();

    public string Execute(string input) {
        var parts = input.Trim().Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";
        var cmd = parts[0];
        var arg1 = parts.Length > 1 ? parts[1] : "";
        var arg2 = parts.Length > 2 ? parts[2] : "";

        return cmd switch {
            "pwd" => fs.Pwd(),
            "ls" => fs.Ls(),
            "cd" => fs.Cd(arg1),
            "mkdir" => fs.Mkdir(arg1),
            "touch" => fs.Touch(arg1),
            "rm" => fs.Rm(arg1),
            "cat" => fs.Cat(arg1),
            "write" => fs.Write(arg1, arg2),
            _ => $"Unknown command: {cmd}"
        };
    }
}