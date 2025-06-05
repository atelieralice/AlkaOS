using AlkaOS.Kernel.Scheduling;
using Godot;
using System;
using System.Collections.Generic;

namespace AlkaOS.Kernel.Scheduling;

public interface IScheduler {
    void AddProcess ( PCB process );
    void RemoveProcess ( int pid );
    PCB GetNextProcess ( );
    IEnumerable<PCB> GetReadyQueue ( );
}

public class RoundRobinScheduler : IScheduler {
    void AddProcess ( PCB process ) {
        
    }
    void RemoveProcess ( int pid );
    PCB GetNextProcess ( );
    IEnumerable<PCB> GetReadyQueue ( );
}