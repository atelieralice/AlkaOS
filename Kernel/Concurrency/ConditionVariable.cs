using System.Collections.Generic;
using AlkaOS.Kernel.Threading;

namespace AlkaOS.Kernel.Concurrency;

public class ConditionVariable
{
    private Queue<SimThread> waitingThreads = new();

    public void Wait(Lock lck, SimThread thread)
    {
        lck.Release();
        waitingThreads.Enqueue(thread);
        thread.State = ThreadState.WAITING;
    }

    public void Signal()
    {
        if (waitingThreads.Count > 0)
        {
            var thread = waitingThreads.Dequeue();
            thread.State = ThreadState.READY;
        }
    }

    public void Broadcast()
    {
        while (waitingThreads.Count > 0)
        {
            var thread = waitingThreads.Dequeue();
            thread.State = ThreadState.READY;
        }
    }

    // Added this while trying to fix a memory leak
    public void Clear()
    {
        while (waitingThreads.Count > 0)
        {
            var thread = waitingThreads.Dequeue();
            thread.State = ThreadState.READY;
        }
    }
}