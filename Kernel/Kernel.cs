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
        allProcesses.Add ( pcb );
        scheduler.AddProcess ( pcb );
    }

    public void SwitchProcess ( ) {
        foreach ( var pcb in allProcesses ) {
            if ( pcb.State == ProcessState.RUNNING ) {
                pcb.State = ProcessState.READY;
            }

            PCB next = scheduler.GetNextProcess ( );
            if ( next != null ) {
                next.State = ProcessState.RUNNING;
            }
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
