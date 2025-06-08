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

    private PCB runningProcess = null;
    private int runningQuantumLeft = 0;

    public MLFQScheduler ( int numQueues = 3, int[] quanta = null, int boostInterval = 100 ) {
        this.numQueues = numQueues;
        queues = new List<Queue<PCB>> ( );
        for ( int i = 0; i < numQueues; i++ )
            queues.Add ( new Queue<PCB> ( ) );
        timeQuanta = quanta ?? new int[] { 10, 20, 40 }; // 4, 8, 16 
        this.boostInterval = boostInterval;
    }

    public void AddProcess ( PCB process ) {
        process.QueueLevel = 0;
        process.TimeUsedAtLevel = 0;
        process.State = ProcessState.READY;
        queues[0].Enqueue ( process );
    }

    public void RemoveProcess ( int pid ) {
        foreach ( var queue in queues ) {
            var temp = new Queue<PCB> ( );
            while ( queue.Count > 0 ) {
                var pcb = queue.Dequeue ( );
                if ( pcb.ProcessID != pid )
                    temp.Enqueue ( pcb );
            }
            while ( temp.Count > 0 )
                queue.Enqueue ( temp.Dequeue ( ) );
        }
        if (runningProcess != null && runningProcess.ProcessID == pid)
            runningProcess = null;
    }

    // Call this every tick (e.g. from Kernel or Node2D _PhysicsProcess)
    public void Tick() {
        ticksSinceBoost++;
        if (ticksSinceBoost >= boostInterval) {
            BoostAllProcesses();
            ticksSinceBoost = 0;
        }

        if (runningProcess == null) {
            runningProcess = GetNextProcess();
            if (runningProcess != null)
                runningQuantumLeft = timeQuanta[runningProcess.QueueLevel];
        }

        if (runningProcess != null) {
            runningProcess.TimeUsedAtLevel++;
            runningQuantumLeft--;

            if (runningQuantumLeft <= 0) {
                // Demote if not at lowest queue
                int level = runningProcess.QueueLevel;
                if (level < numQueues - 1) {
                    runningProcess.QueueLevel++;
                }
                runningProcess.TimeUsedAtLevel = 0;
                queues[runningProcess.QueueLevel].Enqueue(runningProcess);
                runningProcess = null;
            }
        }
    }

    // Returns the currently running process
    public PCB GetCurrentProcess() {
        return runningProcess;
    }

    // Used to get the next process when needed (e.g. after termination)
    public PCB GetNextProcess() {
        for (int i = 0; i < numQueues; i++) {
            if (queues[i].Count > 0) {
                var pcb = queues[i].Dequeue();
                return pcb;
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

    public void ForceSwitch() {
        runningProcess = null;
    }

    public List<Queue<PCB>> GetQueues()
    {
        return queues;
    }
}