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
    private readonly Queue<PCB> readyQueue = new Queue<PCB> ( );

    public int TimeQuantum { get; set; } = 2;

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
        readyQueue.Clear ( );
        while ( tempQueue.Count > 0 )
            readyQueue.Enqueue ( tempQueue.Dequeue ( ) );
    }

    public PCB GetNextProcess ( ) { // possible edge case
        if ( readyQueue.Count == 0 )
            return null;
        PCB pcb = readyQueue.Dequeue ( );
        readyQueue.Enqueue ( pcb );
        return pcb;
    }

    public IEnumerable<PCB> GetReadyQueue ( ) {
        return readyQueue.ToArray ( );
    }
}

// public class MLFQScheduler : IScheduler {
//     private readonly List<Queue<PCB>> queues;
//     private readonly int[] timeQuanta;
//     private readonly int numQueues;
//     private readonly int boostInterval;
//     private int ticksSinceBoost = 0;

//     public MLFQScheduler ( int numQueues = 3, int[] quanta = null, int boostInterval = 100 ) {
//         this.numQueues = numQueues;
//         queues = new List<Queue<PCB>> ( );
//         for ( int i = 0; i < numQueues; i++ )
//             queues.Add ( new Queue<PCB> ( ) );
//         timeQuanta = quanta ?? new int[] {10, 20, 40};
//         this.boostInterval = boostInterval;
//     }

//     public void AddProcess ( PCB process ) {
//         queues[0].Enqueue ( process );
//     }

//     public void RemoveProcess ( int pid ) {
//         // Remove process from all queues
//         foreach ( var queue in queues ) {
//             var temp = new Queue<PCB> ( );
//             while ( queue.Count > 0 ) {
//                 var pcb = queue.Dequeue ( );
//                 if ( pcb.ProcessID != pid )
//                     temp.Enqueue ( pcb );
//             }
//             while ( temp.Count > 0 )
//                 queue.Enqueue ( temp.Dequeue ( ) );
//         }
//     }

//     public PCB GetNextProcess ( ) {
//         // Always pick from the highest non-empty queue
//         for ( int i = 0; i < numQueues; i++ ) {
//             if ( queues[i].Count > 0 )
//                 return queues[i].Peek ( ); // For now, just peek; you'll want to handle time slices and demotion later
//         }
//         return null;
//     }

//     public IEnumerable<PCB> GetReadyQueue ( ) {
//         foreach ( var queue in queues )
//             foreach ( var pcb in queue )
//                 yield return pcb;
//     }
// }