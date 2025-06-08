using AlkaOS.Kernel.Scheduling;
using Godot;
using System;
using System.Collections.Generic;

namespace AlkaOS.Kernel;

public partial class Kernel : Node2D {
    [Signal]
    public delegate void ProcessCreatedEventHandler ( int pid, string name, int Priority );

    // Make fields readonly where appropriate and use object initializers
    private readonly MLFQScheduler scheduler = new MLFQScheduler ( );
    private readonly List<PCB> allProcesses = new ( );
    private readonly MemoryManager memoryManager = new ( );

    public IEnumerable<PCB> GetAllProcesses ( ) => allProcesses;

    // Public getter for MemoryManager so visualizers can access it
    public MemoryManager GetMemoryManager ( ) => memoryManager;

    public void CreateProcess ( int pid, string name, int priority ) {
        var pcb = new PCB ( pid, name, priority ) {
            PageTable = memoryManager.AllocatePages ( 4, pid )
        };
        allProcesses.Add ( pcb );
        scheduler.AddProcess ( pcb );
        // Let ProcessVisualizer know when a process is created
        EmitSignal ( SignalName.ProcessCreated, pid, name, priority );
    }

    public void SwitchProcess ( ) {
        // Set the currently running process to READY
        var running = scheduler.GetCurrentProcess ( );
        if ( running != null ) {
            running.State = ProcessState.READY;
        }
        // Force scheduler to pick a new process on next tick
        scheduler.ForceSwitch ( );
    }

    public void TerminateProcess ( int pid ) {
        var pcb = allProcesses.Find ( p => p.ProcessID == pid );
        if ( pcb is null ) throw new Exception ( "Process not found!" );
        pcb.State = ProcessState.TERMINATED;
        scheduler.RemoveProcess ( pid );
        memoryManager.FreePages ( pcb.PageTable );
    }

    // Address translation for a process
    public int TranslateProcessAddress ( int pid, int virtualAddress ) {
        var pcb = allProcesses.Find ( p => p.ProcessID == pid )
            ?? throw new Exception ( "Process not found!" );
        return memoryManager.TranslateAddress ( pcb.PageTable, virtualAddress );
    }

    public override void _PhysicsProcess ( double delta ) {
        scheduler.Tick ( );
        var running = scheduler.GetCurrentProcess ( );
        foreach ( var pcb in allProcesses ) {
            pcb.State = ( pcb == running ) ? ProcessState.RUNNING : ( pcb.State != ProcessState.TERMINATED ? ProcessState.READY : ProcessState.TERMINATED );
        }
    }
}
