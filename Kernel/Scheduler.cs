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

public class RoundRobinScheduler : IScheduler { // DEPRECATED
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

public class MLFQScheduler : IScheduler {
    private readonly List<Queue<PCB>> queues;
    private readonly int[] timeQuanta;
    private readonly int numQueues;
    private readonly int boostInterval;
    private int ticksSinceBoost = 0;

    public MLFQScheduler(int numQueues = 3, int[] quanta = null, int boostInterval = 100) {
        this.numQueues = numQueues;
        queues = new List<Queue<PCB>>();
        for (int i = 0; i < numQueues; i++)
            queues.Add(new Queue<PCB>());
        timeQuanta = quanta ?? new int[] { 10, 20, 40 };
        this.boostInterval = boostInterval;
    }

    public void AddProcess(PCB process) {
        process.QueueLevel = 0;
        process.TimeUsedAtLevel = 0;
        process.State = ProcessState.READY;
        queues[0].Enqueue(process);
    }

    public void RemoveProcess(int pid) {
        foreach (var queue in queues) {
            var temp = new Queue<PCB>();
            while (queue.Count > 0) {
                var pcb = queue.Dequeue();
                if (pcb.ProcessID != pid)
                    temp.Enqueue(pcb);
            }
            while (temp.Count > 0)
                queue.Enqueue(temp.Dequeue());
        }
    }

    public PCB GetNextProcess() {
        // Priority boost if needed
        ticksSinceBoost++;
        if (ticksSinceBoost >= boostInterval) {
            BoostAllProcesses();
            ticksSinceBoost = 0;
        }

        for (int i = 0; i < numQueues; i++) {
            if (queues[i].Count > 0) {
                var pcb = queues[i].Dequeue();

                // Simulate running for one time unit
                pcb.TimeUsedAtLevel++;

                // If process used up its time slice at this level, demote if not at lowest queue
                if (pcb.TimeUsedAtLevel >= timeQuanta[i]) {
                    if (i < numQueues - 1) {
                        pcb.QueueLevel = i + 1;
                        pcb.TimeUsedAtLevel = 0;
                        queues[i + 1].Enqueue(pcb);
                    } else {
                        // Already at lowest queue, just re-enqueue
                        pcb.TimeUsedAtLevel = 0;
                        queues[i].Enqueue(pcb);
                    }
                    return GetNextProcess(); // Try next process in this queue
                } else {
                    // Not used up time slice, re-enqueue at same level
                    queues[i].Enqueue(pcb);
                    return pcb;
                }
            }
        }
        return null;
    }

    public IEnumerable<PCB> GetReadyQueue() {
        foreach (var queue in queues)
            foreach (var pcb in queue)
                yield return pcb;
    }

    private void BoostAllProcesses() {
        // Move all processes to the top queue and reset their time used
        for (int i = 1; i < numQueues; i++) {
            while (queues[i].Count > 0) {
                var pcb = queues[i].Dequeue();
                pcb.QueueLevel = 0;
                pcb.TimeUsedAtLevel = 0;
                queues[0].Enqueue(pcb);
            }
        }
    }
}