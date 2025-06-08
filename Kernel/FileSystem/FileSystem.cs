#nullable enable
using System;
using System.Collections.Generic;

namespace AlkaOS.Kernel.FileSystem;

public class FileEntry {
    public string Name { get; set; }
    public string Content { get; set; }
    public FileEntry(string name) {
        Name = name;
        Content = "";
    }
}

public class DirectoryEntry {
    public string Name { get; set; }
    public Dictionary<string, DirectoryEntry> Directories { get; set; } = new();
    public Dictionary<string, FileEntry> Files { get; set; } = new();
    public DirectoryEntry? Parent { get; set; }
    public DirectoryEntry(string name, DirectoryEntry? parent = null) {
        Name = name;
        Parent = parent;
    }
}

public class SimpleFileSystem {
    private DirectoryEntry root = new DirectoryEntry("/");
    private DirectoryEntry current;

    public SimpleFileSystem() {
        current = root;
    }

    public string Pwd() => GetPath(current);

    public string Ls() {
        var dirs = string.Join("  ", current.Directories.Keys);
        var files = string.Join("  ", current.Files.Keys);
        return $"{dirs}{(dirs.Length > 0 && files.Length > 0 ? "  " : "")}{files}";
    }

    public string Cd(string name) {
        if (name == "/") { current = root; return ""; }
        if (name == "..") {
            if (current.Parent != null) current = current.Parent;
            return "";
        }
        if (current.Directories.TryGetValue(name, out var dir)) {
            current = dir;
            return "";
        }
        return $"No such directory: {name}";
    }

    public string Mkdir(string name) {
        if (current.Directories.ContainsKey(name)) return "Directory already exists.";
        current.Directories[name] = new DirectoryEntry(name, current);
        return "";
    }

    public string Touch(string name) {
        if (current.Files.ContainsKey(name)) return "File already exists.";
        current.Files[name] = new FileEntry(name);
        return "";
    }

    public string Rm(string name) {
        if (current.Files.Remove(name)) return "";
        if (current.Directories.Remove(name)) return "";
        return "No such file or directory.";
    }

    public string Cat(string name) {
        if (current.Files.TryGetValue(name, out var file)) return file.Content;
        return "No such file.";
    }

    public string Write(string name, string content) {
        if (!current.Files.TryGetValue(name, out var file)) return "No such file.";
        file.Content = content;
        return "";
    }

    private string GetPath(DirectoryEntry dir) {
        if (dir.Parent == null) return "/";
        var stack = new Stack<string>();
        var cur = dir;
        while (cur.Parent != null) {
            stack.Push(cur.Name);
            cur = cur.Parent;
        }
        return "/" + string.Join("/", stack);
    }
}