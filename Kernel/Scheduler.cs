using AlkaOS.Kernel.Scheduling;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AlkaOS.Kernel.Scheduling;

public interface IScheduler {
    void AddProcess ( PCB process );
    void RemoveProcess ( int pid );
    PCB GetNextProcess ( );
    IEnumerable<PCB> GetReadyQueue ( );
}

public class RoundRobinScheduler : IScheduler {
    public readonly Queue<PCB> readyQueue = new Queue<PCB>();

    public void AddProcess ( PCB process ) {
        readyQueue.Enqueue ( process );
    }

    public void RemoveProcess ( int pid ) {
        Queue<PCB> tempQueue = new Queue<PCB> ( );
        while ( readyQueue.Count > 0 ) {
            PCB pcb = readyQueue.Dequeue ( );
            if ( pcb.ProcessID != pid )
                tempQueue.Enqueue ( pcb );
        }
        readyQueue.Clear();
        while (tempQueue.Count > 0)
            readyQueue.Enqueue(tempQueue.Dequeue());
    }

    public PCB GetNextProcess ( ) {
        if ( readyQueue.Count == 0 )
            return null;
        PCB pcb = readyQueue.Dequeue();
        readyQueue.Enqueue ( pcb );
        return pcb;
    }

    public IEnumerable<PCB> GetReadyQueue ( ) {
        return readyQueue.ToArray ( );
    }
}