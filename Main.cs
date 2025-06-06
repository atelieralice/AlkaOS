using Godot;
using System;
using System.Threading.Tasks;
using AlkaOS.Kernel;
using AlkaOS.Kernel.Scheduling;
using System.Linq;

public partial class Main : Node2D {
    private Kernel kernel;

    public override void _Ready() {
        kernel = GetNode<Kernel>("Kernel");
        _ = RunDebugSequence();
    }

    private async Task RunDebugSequence() {
        // Add first process
        kernel.CreateProcess(5234, "Firefox", 3);
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Add second process
        kernel.CreateProcess(305, "Zoom", 2);
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Switch before adding more
        kernel.SwitchProcess();
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Add third process later
        kernel.CreateProcess(4525, "Spotify", 5);
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Add fourth process even later
        kernel.CreateProcess(1001, "VSCode", 1);
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Switch and terminate in between
        kernel.SwitchProcess();
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        kernel.TerminateProcess(5234);
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Add another process after some terminations
        kernel.CreateProcess(2002, "Terminal", 4);
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        kernel.SwitchProcess();
        WriteProcessInfo.PrintAll(kernel);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        kernel.SwitchProcess();
        WriteProcessInfo.PrintAll(kernel);
        // No need to wait at the end
    }

    public override void _Process(double delta) { }
}