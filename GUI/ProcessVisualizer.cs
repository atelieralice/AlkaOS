using Godot;
using System;
using System.Collections.Generic;

public partial class ProcessVisualizer : Node2D {
	private AlkaOS.Kernel.Kernel kernel;
	private List<ColorRect> processRects = new List<ColorRect> ( );
	private int rectWidth = 120;
	private int rectHeight = 48;
	private int margin = 16;

	public override void _Ready ( ) {
		// Get the sibling node named "Kernel"
		kernel = GetNode<AlkaOS.Kernel.Kernel> ( "%Kernel" );
		kernel.ProcessCreated += OnProcessCreated;
		QueueRedraw ( );
	}

	public override void _ExitTree ( ) {
		// Unsubscribe to avoid memory leaks
		if ( kernel != null )
			kernel.ProcessCreated -= OnProcessCreated;
	}

	public override void _Process ( double delta ) {
	}

	public void OnProcessCreated ( int pid, string name, int priority ) {
		var rect = new ColorRect ( );
		rect.Color = GetColorForPriority ( priority );
		rect.Size = new Vector2 ( rectWidth, rectHeight );

		// Arrange rectangles in a vertical list, centered horizontally
		int index = processRects.Count;
		float x = ( 1152 - rectWidth ) / 2;
		float y = margin + index * ( rectHeight + margin );
		rect.Position = new Vector2 ( x, y );

		// Optional: Add a label for process name and PID
		var label = new Label ( );
		label.Text = $"{name} (PID: {pid})";
		label.Size = rect.Size;
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.VerticalAlignment = VerticalAlignment.Center;
		rect.AddChild ( label );

		AddChild ( rect );
		processRects.Add ( rect );
	}

	private Color GetColorForPriority ( int priority ) {
		// Assign a color based on priority (example: lower = greener, higher = redder)
		float t = Mathf.Clamp ( priority / 10.0f, 0, 1 );
		return new Color ( 1.0f * t, 1.0f - t, 0.4f + 0.3f * ( 1 - t ) );
	}
}