using System.Collections.Generic;
using AlkaOS.Kernel.Threading;

namespace AlkaOS.Kernel.Concurrency;

public class Lock {
    private SimThread owner = null;
    private Queue<SimThread> waitingThreads = new ( );

    public bool IsLocked => owner != null;

    public void Acquire ( SimThread thread ) {
        if ( owner == null ) {
            owner = thread;
        } else {
            waitingThreads.Enqueue ( thread );
            thread.State = ThreadState.WAITING;
        }
    }

    public void Release ( ) {
        if ( waitingThreads.Count > 0 ) {
            owner = waitingThreads.Dequeue ( );
            owner.State = ThreadState.READY;
        } else {
            owner = null;
        }
    }

    // Add this method for cleanup
    public void Clear() {
        // Set all waiting threads to READY before clearing
        while (waitingThreads.Count > 0) {
            var thread = waitingThreads.Dequeue();
            thread.State = ThreadState.READY;
        }
        owner = null;
    }
}