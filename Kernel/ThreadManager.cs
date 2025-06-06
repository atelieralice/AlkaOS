using System.Linq;

namespace AlkaOS.Kernel.Threading;

public static class ThreadManager {
    // Create a new thread for a given process
    public static SimThread CreateThread ( PCB pcb ) {
        int newThreadId = pcb.Threads.Count > 0 ? pcb.Threads.Max ( t => t.ThreadID ) + 1 : 1;
        var thread = new SimThread ( newThreadId );
        thread.State = ThreadState.READY;
        pcb.Threads.Add ( thread );
        return thread;
    }

    // Terminate a thread by ID for a given process
    public static bool TerminateThread ( PCB pcb, int threadId ) {
        var thread = pcb.Threads.FirstOrDefault ( t => t.ThreadID == threadId );
        if ( thread != null ) {
            thread.State = ThreadState.TERMINATED;
            return true;
        }
        return false;
    }
}
