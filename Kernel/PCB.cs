using System.Collections.Generic;
using AlkaOS.Kernel.Threading;

namespace AlkaOS.Kernel;

public enum ProcessState
{
    NEW,
    READY,
    RUNNING,
    WAITING,
    TERMINATED
}

public class PCB
{
    public int ProcessID { get; }
    public string ProcessName { get; set; }
    public ProcessState State { get; set; }
    public int Priority { get; set; }
    public int QueueLevel { get; set; } = 0;
    public int TimeUsedAtLevel { get; set; } = 0;
    public Dictionary<int, int> PageTable { get; set; } // virtual page -> physical frame
    public List<SimThread> Threads { get; set; }

    public PCB(int pid, string name, int priority, int parentPid = -1)
    {
        ProcessID = pid;
        ProcessName = name;
        Priority = priority;
        State = ProcessState.NEW;
        PageTable = new Dictionary<int, int>();
        Threads = new List<SimThread>();
    }
}
