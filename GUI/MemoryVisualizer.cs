using Godot;
using System;

public partial class MemoryVisualizer : Node2D {
    private AlkaOS.Kernel.MemoryManager memoryManager;

    // Visualization settings
    private int cellSize = 24; // Size of each memory cell (rectangle)
    private int columns = 32;  // 1152 / 36 = 32 columns (adjust as needed)
    private int rows = 8;      // 256 frames / 32 columns = 8 rows

    public override void _Ready ( ) {
        // Get the sibling node named "Kernel" and access its MemoryManager
        var kernel = GetNode<AlkaOS.Kernel.Kernel> ( "%Kernel" );
        memoryManager = kernel.GetMemoryManager ( );
        memoryManager.MemoryChanged += OnMemoryChanged;
        QueueRedraw ( );
    }

    public override void _ExitTree ( ) {
        // Unsubscribe to avoid memory leaks
        if ( memoryManager != null )
            memoryManager.MemoryChanged -= OnMemoryChanged;
    }

    public override void _Process ( double delta ) {
    }

    public void OnMemoryChanged ( ) {
        QueueRedraw ( );
    }

    public override void _Draw ( ) {
        if ( memoryManager == null || memoryManager.FrameOwners == null )
            return;

        var owners = memoryManager.FrameOwners;
        for ( int i = 0; i < owners.Length; i++ ) {
            int x = ( i % columns ) * cellSize;
            int y = ( i / columns ) * cellSize;
            Color color = owners[i] == -1 ? Colors.Gray : GetColorForProcess ( owners[i] );
            DrawRect ( new Rect2 ( x, y, cellSize - 2, cellSize - 2 ), color );
        }
    }

    private Color GetColorForProcess ( int pid ) {
        // Simple hash for color, or use a dictionary for consistent colors
        RandomNumberGenerator rng = new RandomNumberGenerator ( );
        rng.Seed = (ulong)pid;
        return new Color ( rng.RandfRange ( 0.2f, 1f ), rng.RandfRange ( 0.2f, 1f ), rng.RandfRange ( 0.2f, 1f ) );
    }
}