using Godot;

namespace AlkaOS.GUI;

public partial class MLFQVisualizer : Node2D
{
    private AlkaOS.Kernel.Kernel kernel;
    private AlkaOS.Kernel.Scheduling.MLFQScheduler scheduler;

    private int rectSize = 20;
    private int margin = 8;
    private int queueMargin = 32;

    public override void _Ready()
    {
        kernel = GetNode<AlkaOS.Kernel.Kernel>("%Kernel");
        // Black magic.
        scheduler = kernel.GetType().GetField("scheduler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(kernel) as AlkaOS.Kernel.Scheduling.MLFQScheduler;
        Position = new Vector2(40, 223);
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (scheduler == null)
            return;

        // Get all queues and draw them
        var queues = scheduler.GetQueues();
        for (int q = 0; q < queues.Count; q++)
        {
            var queue = queues[q];
            var labelText = $"Queue {q}";
            var labelPos = new Vector2(0, q * (rectSize + queueMargin) - 6);
            var labelColor = new Color("#F5F5DC"); // Beige color

            // Draw very thin black outline (just 4 directions)
            int outlineThickness = 1;
            foreach (var offset in new[] {
                new Vector2(-outlineThickness, 0),
                new Vector2(outlineThickness, 0),
                new Vector2(0, -outlineThickness),
                new Vector2(0, outlineThickness)
            })
            {
                DrawString(
                    GetFont(),
                    labelPos + offset,
                    labelText,
                    HorizontalAlignment.Left,
                    -1,
                    12,
                    Colors.Black
                );
            }

            // Draw
            DrawString(
                GetFont(),
                labelPos,
                labelText,
                HorizontalAlignment.Left,
                -1,
                12,
                labelColor
            );

            int i = 0;
            foreach (var pcb in queue)
            {
                float x = i * (rectSize + margin);
                float y = q * (rectSize + queueMargin);
                var rect = new Rect2(x, y, rectSize, rectSize);
                DrawRect(rect, GetColorForPriority(pcb.Priority));
                DrawString(GetFont(), new Vector2(x + 3, y + rectSize - 5), pcb.ProcessID.ToString(), HorizontalAlignment.Left, -1, 8, Colors.Black);
                i++;
            }
        }
    }

    private static Font GetFont()
    {
        return ThemeDB.FallbackFont;
    }

    private static Color GetColorForPriority(int priority)
    {
        float t = Mathf.Clamp(priority / 10.0f, 0, 1);
        return new Color(1.0f * t, 1.0f - t, 0.4f + 0.3f * (1 - t));
    }
}