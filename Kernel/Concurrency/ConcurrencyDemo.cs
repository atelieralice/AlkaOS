using Godot;
using System;
using System.Collections.Generic;
using AlkaOS.Kernel.Threading;

namespace AlkaOS.Kernel.Concurrency;

public class ReadersWritersDemo {
    private int readers = 0;
    private int waitingWriters = 0; // Track waiting writers
    private bool writerActive = false;
    private readonly Lock rwLock = new ( );
    private readonly ConditionVariable canRead = new ( );
    private readonly ConditionVariable canWrite = new ( );

    // Reader entry
    public void StartRead ( SimThread thread ) {
        thread.WaitingReason = "reader"; // For visualizer
        rwLock.Acquire ( thread );
        while ( writerActive || waitingWriters > 0 ) { // Block if any writer is waiting
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
        waitingWriters++;
        while ( writerActive || readers > 0 ) {
            canWrite.Wait ( rwLock, thread );
            rwLock.Acquire ( thread ); // reacquire after wait
        }
        waitingWriters--;
        writerActive = true;
        rwLock.Release ( );
    }

    public bool TryStartWrite(SimThread thread)
    {
        thread.WaitingReason = "writer";
        rwLock.Acquire(thread);
        if (writerActive || readers > 0)
        {
            canWrite.Wait(rwLock, thread);
            rwLock.Release();
            return false; // Could not start writing
        }
        waitingWriters--;
        writerActive = true;
        rwLock.Release();
        return true; // Started writing
    }

    // Writer exit
    public void EndWrite ( SimThread thread ) {
        rwLock.Acquire ( thread );
        writerActive = false;
        canRead.Broadcast ( ); // let all waiting readers in
        canWrite.Signal ( );   // or let another writer in
        rwLock.Release ( );
    }

    public void Clear() {
        rwLock.Clear();
        canRead.Clear();
        canWrite.Clear();
        readers = 0;
        waitingWriters = 0;
        writerActive = false;
    }
}

public partial class ConcurrencyDemo : Button
{
    private Queue<Action> demoSteps = new();
    private bool isRunning = false;
    private float stepTimer = 0f;
    private float stepInterval = 1.0f; // seconds

    // Store the demo instance as a field so we can clear it later
    private ReadersWritersDemo demo;

    public override void _Pressed()
    {
        if (!isRunning)
            StartDemo();
    }

    private void StartDemo()
    {
        demoSteps.Clear();
        var demoLog = GetNodeOrNull<RichTextLabel>("%ConsoleOutput");
        if (demoLog != null)
            demoLog.Text = "";

        demo = new ReadersWritersDemo();
        demo.Clear();

        var reader1 = new SimThread(1);
        var reader2 = new SimThread(2);
        var writer1 = new SimThread(3);

        demoSteps.Enqueue(() => AppendDemoLog("Reader 1 wants to read."));
        demoSteps.Enqueue(() => { demo.StartRead(reader1); AppendDemoLog("Reader 1 is READING."); });
        demoSteps.Enqueue(() => AppendDemoLog("Reader 2 wants to read."));
        demoSteps.Enqueue(() => { demo.StartRead(reader2); AppendDemoLog("Reader 2 is READING."); });
        demoSteps.Enqueue(() => { demo.EndRead(reader1); AppendDemoLog("Reader 1 finished reading."); });
        demoSteps.Enqueue(() => AppendDemoLog("Writer 1 wants to write."));
        demoSteps.Enqueue(() => TryStartWriterStep(writer1));
        demoSteps.Enqueue(() => { demo.EndRead(reader2); AppendDemoLog("Reader 2 finished reading."); });

        isRunning = true;
        stepTimer = 0f;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!isRunning || demoSteps.Count == 0)
            return;

        stepTimer += (float)delta;
        if (stepTimer >= stepInterval)
        {
            stepTimer = 0f;
            var step = demoSteps.Dequeue();
            step.Invoke();

            if (demoSteps.Count == 0)
            {
                isRunning = false;
                // CLEANUP: Clear all queues to release references and prevent leaks
                demo?.Clear();
                demo = null;
            }
        }
    }

    private void AppendDemoLog(string message)
    {
        var demoLog = GetNodeOrNull<RichTextLabel>("%ConsoleOutput");
        if (demoLog != null)
            demoLog.Text += (demoLog.Text.Length > 0 ? "\n" : "") + message;
        else
            GD.Print(message);
    }

    private void TryStartWriterStep(SimThread writer)
    {
        if (demo.TryStartWrite(writer))
        {
            AppendDemoLog("Writer 1 is WRITING.");
            demoSteps.Enqueue(() => { demo.EndWrite(writer); AppendDemoLog("Writer 1 finished writing."); });
        }
        else
        {
            AppendDemoLog("Writer 1 is WAITING (lock held by reader).");
            demoSteps.Enqueue(() => TryStartWriterStep(writer)); // Try again next tick
        }
    }
}