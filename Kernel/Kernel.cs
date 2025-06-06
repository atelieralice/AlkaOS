using AlkaOS.Kernel.Scheduling;
using Godot;
using System;
using System.Collections.Generic;

namespace AlkaOS.Kernel;

public partial class Kernel : Node2D {
    [Signal]
    public delegate void ProcessCreatedEventHandler ( int pid, string name, int Priority );

    private IScheduler scheduler = new MLFQScheduler ( );
    private List<PCB> allProcesses = [];
    private MemoryManager memoryManager = new MemoryManager ( );

    public IEnumerable<PCB> GetAllProcesses ( ) => allProcesses;

    // Public getter for MemoryManager so visualizers can access it
    public MemoryManager GetMemoryManager ( ) => memoryManager;

    public void CreateProcess ( int pid, string name, int priority ) {
        PCB pcb = new PCB ( pid, name, priority );
        // Allocate 4 pages per process as an example, pass processId for visualization
        pcb.PageTable = memoryManager.AllocatePages ( 4, pid );
        allProcesses.Add ( pcb );
        scheduler.AddProcess ( pcb );
        // Let ProcessVisualizer know when a process is created
        EmitSignal ( SignalName.ProcessCreated, pid, name, priority );
    }
    public void SwitchProcess ( ) {
        // Set the currently running process to READY
        var running = allProcesses.Find ( pcb => pcb.State == ProcessState.RUNNING );
        if ( running != null ) {
            running.State = ProcessState.READY;
        }
        // Get the next process from the scheduler and set it to RUNNING
        PCB next = scheduler.GetNextProcess ( );
        if ( next != null ) {
            next.State = ProcessState.RUNNING;
        }
    }
    public void TerminateProcess ( int pid ) {
        PCB pcb = allProcesses.Find ( p => p.ProcessID == pid );
        if ( pcb != null ) {
            pcb.State = ProcessState.TERMINATED;
            scheduler.RemoveProcess ( pid );
            memoryManager.FreePages ( pcb.PageTable );
        }
    }

    // Address translation for a process
    public int TranslateProcessAddress ( int pid, int virtualAddress ) {
        PCB pcb = allProcesses.Find ( p => p.ProcessID == pid );
        if ( pcb == null ) throw new Exception ( "Process not found!" );
        return memoryManager.TranslateAddress ( pcb.PageTable, virtualAddress );
    }
}
