// The logic is moved to ConcurrencyDemo.cs
// Not used
using Godot;
using System.Threading.Tasks;

public partial class RunConcurrencyDemo : Button {
    private string demoLogContents = "";

    public override void _Pressed() {
        _ = RunDemo();
    }

    private async Task RunDemo() {
        var demo = new AlkaOS.Kernel.Concurrency.ReadersWritersDemo();
        var reader1 = new AlkaOS.Kernel.Threading.SimThread(1);
        var reader2 = new AlkaOS.Kernel.Threading.SimThread(2);
        var writer1 = new AlkaOS.Kernel.Threading.SimThread(3);

        SetDemoLog("Reader 1 wants to read.");
        demo.StartRead(reader1);
        SetDemoLog("Reader 1 is READING.");
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        SetDemoLog("Reader 2 wants to read.");
        demo.StartRead(reader2);
        SetDemoLog("Reader 2 is READING.");
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        SetDemoLog("Reader 1 finished reading.");
        demo.EndRead(reader1);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        SetDemoLog("Writer 1 wants to write.");
        demo.StartWrite(writer1);
        SetDemoLog("Writer 1 is WRITING.");
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        SetDemoLog("Reader 2 finished reading.");
        demo.EndRead(reader2);
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        SetDemoLog("Writer 1 finished writing.");
        demo.EndWrite(writer1);
    }

    private void SetDemoLog(string message) {
        var demoLog = GetNodeOrNull<RichTextLabel>("%DemoLog");
        if (demoLog != null) {
            if (!string.IsNullOrEmpty(demoLog.Text))
                demoLog.Text += "\n";
            demoLog.Text += message;
        } else {
            GD.Print(message);
        }
    }
}