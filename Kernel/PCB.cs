using Godot;
using System;
using System.Collections.Generic;

namespace AlkaOS.Kernel;

public enum ProcessState {
    NEW,
    READY,
    RUNNING,
    WAITING,
    TERMINATED
}

public class PCB {
    public int ProcessID { get; }
    public string ProcessName { get; set; }
    public ProcessState State { get; set; }
    public int Priority { get; set; }
    public int ProgramCounter { get; set; }
    public int StackPointer { get; set; }
    public Dictionary<string, int> Registers { get; private set; }
    public int ParentPID { get; set; }
    public DateTime CreationTime { get; }
    public int CpuBurstTime { get; set; } // Remaining CPU time
    public int CpuTimeUsed { get; set; } // Used CPU time
    public int ExitCode { get; set; } = 0;
    public string WaitingReason { get; set; }
    public int QueueLevel { get; set; } = 0;
    public int TimeUsedAtLevel { get; set; } = 0;

    public PCB ( int pid, string name, int priority, int parentPid = -1 ) {
        ProcessID = pid;
        ProcessName = name;
        Priority = priority;
        ParentPID = parentPid;
        State = ProcessState.NEW;
        CreationTime = DateTime.Now;

        Registers = new Dictionary<string, int> {
            { "EAX", 0 },
            { "EBX", 0 },
            { "ECX", 0 },
            { "EDX", 0 }
        };

        CpuBurstTime = 0;
        CpuTimeUsed = 0;
        WaitingReason = string.Empty;
    }
}
