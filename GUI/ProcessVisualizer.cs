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

        // Group processes by state (READY, RUNNING, TERMINATED, etc.)
        var processes = kernel.GetAllProcesses().ToList();

        // Define the desired order
        string[] stateOrder = { "READY", "RUNNING", "TERMINATED" };

        // Group by state and order by our custom order, unknown states go last
        var stateGroups = processes
            .GroupBy(p => p.State)
            .OrderBy(g => {
                int idx = System.Array.IndexOf(stateOrder, g.Key.ToString());
                return idx == -1 ? int.MaxValue : idx;
            })
            .ToList();

        float y = margin;
        foreach ( var group in stateGroups ) {
            // Draw queue/state label with smaller font size
            DrawString ( GetFont ( ), new Vector2 ( margin, y - 8 ), group.Key.ToString ( ), HorizontalAlignment.Left, -1, 12, new Color ( 0.8f, 0.8f, 0.8f ) );

            int i = 0;
            foreach ( var pcb in group ) {
                float x = margin + i * ( rectWidth + margin );
                var rect = new Rect2 ( x, y, rectWidth, rectHeight );
                DrawRect ( rect, GetColorForPriority ( pcb.Priority ) );
                // Draw process label with smaller font size
                DrawString ( GetFont ( ), new Vector2 ( x + 8, y + rectHeight / 2 + 6 ), $"{pcb.ProcessName} (PID:{pcb.ProcessID})", HorizontalAlignment.Left, -1, 12, Colors.Black );
                i++;
            }
            y += rectHeight + queueMargin;
        }
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