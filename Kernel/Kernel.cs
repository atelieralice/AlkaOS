using AlkaOS.Kernel.Scheduling;
using Godot;
using System;
using System.Collections.Generic;

namespace AlkaOS.Kernel;

public partial class Kernel : Node2D {
    private IScheduler scheduler = new RoundRobinScheduler ( );
    private List<PCB> allProcesses = [];

    public IEnumerable<PCB> GetAllProcesses ( ) => allProcesses;

    public void CreateProcess ( int pid, string name, int priority ) {
        PCB pcb = new PCB ( pid, name, priority );
        // pcb.State = ProcessState.READY;
        allProcesses.Add ( pcb );
        scheduler.AddProcess ( pcb );
    }
    public void SwitchProcess ( ) {
        // Set the currently running process to READY
        var running = allProcesses.Find(pcb => pcb.State == ProcessState.RUNNING);
        if (running != null) {
            running.State = ProcessState.READY;
        }

        // Get the next process from the scheduler and set it to RUNNING
        PCB next = scheduler.GetNextProcess();
        if (next != null) {
            next.State = ProcessState.RUNNING;
        }
    }

    public void TerminateProcess ( int pid ) {
        PCB pcb = allProcesses.Find ( p => p.ProcessID == pid );
        if ( pcb != null ) {
            pcb.State = ProcessState.TERMINATED;
            scheduler.RemoveProcess ( pid );
        }
    }
}
