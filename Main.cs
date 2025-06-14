using Godot;
using System;
using AlkaOS.Kernel;
using System.Linq;
using AlkaOS.Kernel.FileSystem;

public partial class Main : Node2D
{
    private Kernel kernel;
    private Random random = new Random();
    private FileSystemConsole fsConsole = new();

    public override void _Ready()
    {
        kernel = GetNode<Kernel>("%Kernel");

        // Connect button signals to methods
        var addButton = GetNodeOrNull<Button>("%AddProcess");
        if (addButton != null)
            addButton.Pressed += AddRandomProcess;

        var switchButton = GetNodeOrNull<Button>("%SwitchProcess");
        if (switchButton != null)
            switchButton.Pressed += SwitchToNextProcess;

        var terminateButton = GetNodeOrNull<Button>("%TerminateProcess");
        if (terminateButton != null)
            terminateButton.Pressed += () =>
            {
                var pidEdit = GetNodeOrNull<TextEdit>("%TerminatePID");
                if (pidEdit != null && int.TryParse(pidEdit.Text, out int pid))
                    TerminateProcessByPid(pid);
            };

        var input = GetNode<LineEdit>("%ConsoleInput");
        var output = GetNode<RichTextLabel>("%ConsoleOutput");

        input.TextSubmitted += (string command) =>
        {
            var result = fsConsole.Execute(command);
            // Append command and result to output
            output.Text += $"> {command}\n";
            if (!string.IsNullOrEmpty(result))
                output.Text += result + "\n";
            input.Text = "";
        };
    }

    // private async Task RunDebugSequence() {
    //     kernel.CreateProcess(5234, "Firefox", 3);
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.CreateProcess(305, "Zoom", 2);
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.SwitchProcess();
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.CreateProcess(4525, "Spotify", 5);
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.CreateProcess(1001, "VSCode", 1);
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.SwitchProcess();
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.TerminateProcess(5234);
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.CreateProcess(2002, "Terminal", 4);
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.SwitchProcess();
    //     WriteProcessInfo.PrintAll(kernel);
    //     await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

    //     kernel.SwitchProcess();
    //     WriteProcessInfo.PrintAll(kernel);
    // }

    public void AddRandomProcess()
    {
        var existingPids = kernel.GetAllProcesses().Select(p => p.ProcessID).ToHashSet();
        int pid;
        do
        {
            pid = random.Next(1000, 9999);
        } while (existingPids.Contains(pid));
        string[] names = { "Firefox", "Zoom", "Spotify", "VSCode", "Terminal", "Edge", "WhatsApp", "Chrome", "Explorer" };
        string name = names[random.Next(names.Length)];
        int priority = random.Next(1, 6);
        kernel.CreateProcess(pid, name, priority);
        // WriteProcessInfo.PrintAll(kernel);
    }

    public void SwitchToNextProcess()
    {
        kernel.SwitchProcess();
        // WriteProcessInfo.PrintAll(kernel);
    }

    public void TerminateProcessByPid(int pid)
    {
        kernel.TerminateProcess(pid);
        // WriteProcessInfo.PrintAll(kernel);
    }
}