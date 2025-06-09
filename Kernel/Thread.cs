namespace AlkaOS.Kernel.Threading;

public enum ThreadState
{
    NEW,
    READY,
    RUNNING,
    WAITING,
    TERMINATED
}

public class SimThread
{
    public int ThreadID { get; }
    public ThreadState State { get; set; }
    public int ProgramCounter { get; set; }
    public int StackPointer { get; set; }
    public string WaitingReason { get; set; }

    public SimThread(int threadId)
    {
        ThreadID = threadId;
        State = ThreadState.NEW;
        ProgramCounter = 0;
        StackPointer = 0;
        WaitingReason = string.Empty;
    }
}