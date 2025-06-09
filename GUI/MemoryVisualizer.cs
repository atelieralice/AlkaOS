using Godot;
using System.Collections.Generic;

namespace AlkaOS.GUI;

public partial class MemoryVisualizer : Node2D
{
    private AlkaOS.Kernel.MemoryManager memoryManager;
    private HashSet<int> faultyFrames = new HashSet<int>();
    private HashSet<int> swappedFrames = new HashSet<int>();

    // Visualization settings
    private int cellSize = 24; // Size of each memory cell (rectangle)
    private int columns = 32;  // 1152 / 36 = 32 columns (adjust as needed)
    private int rows = 8;      // 256 frames / 32 columns = 8 rows

    public override void _Ready()
    {
        Position = new Vector2(24, 420);
        // Get the sibling node named "Kernel" and access its MemoryManager
        var kernel = GetNode<AlkaOS.Kernel.Kernel>("%Kernel");
        memoryManager = kernel.GetMemoryManager();
        memoryManager.MemoryChanged += OnMemoryChanged;
        UpdateFrameSets();
        QueueRedraw();
    }

    public override void _ExitTree()
    {
        // Unsubscribe to avoid memory leaks
        if (memoryManager != null)
            memoryManager.MemoryChanged -= OnMemoryChanged;
    }

    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        QueueRedraw();
    }

    public void OnMemoryChanged()
    {
        UpdateFrameSets();
        QueueRedraw();
    }

    private void UpdateFrameSets()
    {
        faultyFrames = memoryManager.GetFaultyFrames();
        swappedFrames = memoryManager.GetSwappedFrames();
    }

    public override void _Draw()
    {
        if (memoryManager == null || memoryManager.FrameOwners == null)
            return;

        var owners = memoryManager.FrameOwners;
        for (int i = 0; i < owners.Length; i++)
        {
            int x = (i % columns) * cellSize;
            int y = (i / columns) * cellSize;
            Color color;
            if (faultyFrames.Contains(i))
                color = Colors.Red;
            else if (swappedFrames.Contains(i))
                color = Colors.Blue;
            else if (owners[i] == -1)
                color = Colors.DarkGray;
            else
                color = GetColorForProcess(owners[i]);
            DrawRect(new Rect2(x, y, cellSize - 2, cellSize - 2), color);
        }
    }

    private Color GetColorForProcess(int pid)
    {
        // Simple hash for color, or use a dictionary for consistent colors
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Seed = (ulong)pid;
        return new Color(rng.RandfRange(0.2f, 1f), rng.RandfRange(0.2f, 1f), rng.RandfRange(0.2f, 1f));
    }
}