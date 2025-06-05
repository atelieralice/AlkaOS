using Godot;
using System;

public partial class ProcessVisualizer : Node2D {
	private AlkaOS.Kernel.Kernel kernel;

	public override void _Ready ( ) {
		// Get the sibling node named "Kernel"
		kernel = GetNode<AlkaOS.Kernel.Kernel> ( "%Kernel" );
		kernel.ProcessCreated += OnProcessCreated;
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
		rect.Color = new Color ( 0.3f, 0.6f, 1.0f ); // TODO: Vary this by priority
		rect.Size = new Vector2 ( 80, 40 );
		rect.Position = new Vector2 ( 0, 0 );
		GD.Print ( "It works" );

		AddChild ( rect );
	}
}