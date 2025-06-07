using System.Collections.Generic;
using AlkaOS.Kernel.Threading;

namespace AlkaOS.Kernel.Concurrency;

public class ReadersWritersDemo {
    private int readers = 0;
    private bool writerActive = false;
    private readonly Lock rwLock = new ( );
    private readonly ConditionVariable canRead = new ( );
    private readonly ConditionVariable canWrite = new ( );

    // Reader entry
    public void StartRead ( SimThread thread ) {
        thread.WaitingReason = "reader"; // For visualizer
        rwLock.Acquire ( thread );
        while ( writerActive ) {
            canRead.Wait ( rwLock, thread );
            rwLock.Acquire ( thread ); // reacquire after wait
        }
        readers++;
        canRead.Signal ( ); // allow other readers
        rwLock.Release ( );
    }

    // Reader exit
    public void EndRead ( SimThread thread ) {
        rwLock.Acquire ( thread );
        readers--;
        if ( readers == 0 )
            canWrite.Signal ( ); // let a writer in
        rwLock.Release ( );
    }

    // Writer entry
    public void StartWrite ( SimThread thread ) {
        thread.WaitingReason = "writer"; // For visualizer
        rwLock.Acquire ( thread );
        while ( writerActive || readers > 0 ) {
            canWrite.Wait ( rwLock, thread );
            rwLock.Acquire ( thread ); // reacquire after wait
        }
        writerActive = true;
        rwLock.Release ( );
    }

    // Writer exit
    public void EndWrite ( SimThread thread ) {
        rwLock.Acquire ( thread );
        writerActive = false;
        canRead.Broadcast ( ); // let all waiting readers in
        canWrite.Signal ( );   // or let another writer in
        rwLock.Release ( );
    }
}