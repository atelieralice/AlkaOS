using Godot;
using System;
using AlkaOS.Kernel;
using AlkaOS.Kernel.Scheduling;
using System.Linq;

public partial class Debug : Node2D {
	private Kernel kernel = new Kernel ( );

	// Called when the node enters the scene tree for the first time.
	public override void _Ready ( ) {
		kernel.CreateProcess ( 5234, "Firefox", 3 );
		kernel.CreateProcess ( 305, "Zoom", 2 );
		kernel.CreateProcess ( 4525, "Spotify", 5 );
		WriteProcessInfo.PrintAll ( kernel );
		GD.Print ( );

		kernel.SwitchProcess ( );
		WriteProcessInfo.PrintAll ( kernel );
		GD.Print ( );

		kernel.SwitchProcess ( );
		WriteProcessInfo.PrintAll ( kernel );
		GD.Print ( );

		kernel.TerminateProcess ( 5234 );
		WriteProcessInfo.PrintAll ( kernel );
		GD.Print ( );

		kernel.SwitchProcess ( );
		WriteProcessInfo.PrintAll ( kernel );
		GD.Print ( );

		kernel.SwitchProcess ( );
		WriteProcessInfo.PrintAll ( kernel );
		GD.Print ( );
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process ( double delta ) {
	}
}

public class WriteProcessInfo { // Write individual process information
	public static void PrintAll ( Kernel kernel ) {
		foreach ( var process in kernel.GetAllProcesses ( ) ) {
			GD.Print (
				$"Process ID: {process.ProcessID}, " +
				$"Name: {process.ProcessName}, " +
				$"State: {process.State}, " +
				$"Priority: {process.Priority} "
			);
		}
	}
}

