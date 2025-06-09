using Godot;
using System.Linq;

namespace AlkaOS.GUI;

public partial class ProcessVisualizer : Node2D {
    private AlkaOS.Kernel.Kernel kernel;

    private int rectWidth = 120;
    private int rectHeight = 48;
    private int margin = 16;
    private int queueMargin = 48;

    public override void _Ready ( ) {
        Position = new Vector2 ( 5, 20 );
        kernel = GetNode<AlkaOS.Kernel.Kernel> ( "%Kernel" );
        kernel.ProcessCreated += OnProcessCreated;
        QueueRedraw ( );
    }

    public override void _ExitTree ( ) {
        if ( kernel != null )
            kernel.ProcessCreated -= OnProcessCreated;
    }

    private void OnProcessCreated ( int pid, string name, int priority ) {
        QueueRedraw ( );
    }

    public override void _Process ( double delta ) { }

    public override void _Draw ( ) {
        if ( kernel == null )
            return;

        var processes = kernel.GetAllProcesses().ToList();

        // Separate processes by state
        var ready = processes.Where(p => p.State.ToString() == "READY").ToList();
        var running = processes.Where(p => p.State.ToString() == "RUNNING").ToList();
        var terminated = processes.Where(p => p.State.ToString() == "TERMINATED").ToList();

        float y = margin;

        // Draw READY queue (top left)
        DrawOutlinedLabel("READY", new Vector2(margin, y - 8));
        for (int i = 0; i < ready.Count; i++) {
            float x = margin + i * (rectWidth + margin);
            var rect = new Rect2(x, y, rectWidth, rectHeight);
            DrawRect(rect, GetColorForPriority(ready[i].Priority));
            DrawString(GetFont(), new Vector2(x + 8, y + rectHeight / 2 + 6), $"{ready[i].ProcessName} (PID:{ready[i].ProcessID})", HorizontalAlignment.Left, -1, 12, new Color("#F5F5DC"));
        }

        // Draw RUNNING queue
        float runningY = y + rectHeight + queueMargin;
        DrawOutlinedLabel("RUNNING", new Vector2(margin, runningY - 8));
        for (int i = 0; i < running.Count; i++) {
            float x = margin + i * (rectWidth + margin);
            var rect = new Rect2(x, runningY, rectWidth, rectHeight);
            DrawRect(rect, GetColorForPriority(running[i].Priority));
            DrawString(GetFont(), new Vector2(x + 8, runningY + rectHeight / 2 + 6), $"{running[i].ProcessName} (PID:{running[i].ProcessID})", HorizontalAlignment.Left, -1, 12, new Color("#F5F5DC"));
        }

        // Draw TERMINATED queue
        float terminatedX = margin + Mathf.Max(running.Count, 1) * (rectWidth + margin);
        DrawOutlinedLabel("TERMINATED", new Vector2(terminatedX, runningY - 8));
        for (int i = 0; i < terminated.Count; i++) {
            float x = terminatedX + i * (rectWidth + margin);
            var rect = new Rect2(x, runningY, rectWidth, rectHeight);
            DrawRect(rect, GetColorForPriority(terminated[i].Priority));
            DrawString(GetFont(), new Vector2(x + 8, runningY + rectHeight / 2 + 6), $"{terminated[i].ProcessName} (PID:{terminated[i].ProcessID})", HorizontalAlignment.Left, -1, 12, new Color("#F5F5DC"));
        }
    }

    // Helper to draw outlined text
    private void DrawOutlinedLabel(string text, Vector2 pos) {
        var labelColor = new Color("#F5F5DC");
        int outlineThickness = 1;
        foreach (var offset in new[] { new Vector2(-outlineThickness, 0), new Vector2(outlineThickness, 0), new Vector2(0, -outlineThickness), new Vector2(0, outlineThickness) }) {
            DrawString(GetFont(), pos + offset, text, HorizontalAlignment.Left, -1, 12, Colors.Black);
        }
        DrawString(GetFont(), pos, text, HorizontalAlignment.Left, -1, 12, labelColor);
    }

    public override void _PhysicsProcess ( double delta ) {
        QueueRedraw ( );
    }

    private static Font GetFont ( ) {
        // Use the default font
        var defaultFont = ThemeDB.FallbackFont;
        return defaultFont;
    }

    private static Color GetColorForPriority ( int priority ) {
        float t = Mathf.Clamp ( priority / 10.0f, 0, 1 );
        return new Color ( 1.0f * t, 1.0f - t, 0.4f + 0.3f * ( 1 - t ) );
    }
}