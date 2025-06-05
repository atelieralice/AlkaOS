using Godot;
using System;
using AlkaOS.Kernel;
using AlkaOS.Kernel.Scheduling;
using System.Linq;

public partial class Debug : Node2D {
	private Kernel kernel = new Kernel ( );

	// Called when the node enters the scene tree for the first time.
	public override void _Ready ( ) { // TODO: automatic pid assignation
		kernel.CreateProcess ( 5234, "Firefox", 3 );
		kernel.CreateProcess ( 305, "Zoom", 2 );
		GD.Print ( "Processes created: " + kernel.GetAllProcesses ( ).Count ( ) );

		// Write individual process information
		foreach ( var process in kernel.GetAllProcesses ( ) ) {
			GD.Print ( $"Process ID: {process.ProcessID}, Name: {process.ProcessName}, State: {process.State}, Priority: {process.Priority}" );
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process ( double delta ) {
	}
}

