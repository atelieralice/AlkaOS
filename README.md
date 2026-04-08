# AlkaOS – Alchemy-Themed OS Simulator

GitHub Repo URL: https://github.com/atelieralice/AlkaOS

![](/AlkaOS_preview.gif "Demonstration")

## Introduction

Our final project in the third-year Operating Systems course was to build an OS simulator with a theme of our choice.

I decided to make an alchemy-themed OS that runs on PCs. The theme leans more toward the fantasy side rather than real alchemy, which made the project feel more approachable and user-friendly, and a bit more fun to work on.

For this project, I used **C#** with **Godot 4.5**. The project runs on **.NET 8**, so I could use modern C# features, and LINQ helped a lot with the parts where I’m basically filtering and querying lists (process lists, queues, etc.). Godot mainly handles the frontend: drawing the visualizers and handling input/UI; while providing a stable tick source via `_PhysicsProcess()`

Although this was the first project I tried to do something with Godot's UI tools, I'm happy with how it turned out.

## Project structure

All of the core logic is written in C#. I tried to keep it modular by putting related systems into their own folders and namespaces.

### Kernel (AlkaOS.Kernel)

This is the main logic of the OS simulation.

- `Kernel/Kernel.cs`: the central controller. Creates/terminates processes, drives scheduling each tick, and coordinates memory.
- `Kernel/MemoryManager.cs`: allocates/frees pages (frames), and also simulates fragmentation + swapping so the memory visualizer has something interesting to show.
- `Kernel/Scheduler.cs`: contains the scheduler interface + implementations. Round Robin is there (mostly as a baseline), and MLFQ is the main one. (Namespace: `AlkaOS.Kernel.Scheduling`)
- `Kernel/PCB.cs`: the Process Control Block (PID, state, priority, queue level, page table, threads, etc.).
- `Kernel/Thread.cs`: a simple simulated thread model and thread states. (Namespace: `AlkaOS.Kernel.Threading`)
- `Kernel/ThreadManager.cs`: helper methods to create/terminate simulated threads. (Namespace: `AlkaOS.Kernel.Threading`)

### Concurrency (AlkaOS.Kernel.Concurrency)

- `Kernel/Concurrency/ConcurrencyDemo.cs`: a readers–writers demo. It logs into the same console output window that the file system uses.
- `Kernel/Concurrency/Lock.cs`: a simple lock primitive for mutual exclusion.
- `Kernel/Concurrency/ConditionVariable.cs`: condition variables for waiting/signaling.

### FileSystem (AlkaOS.Kernel.FileSystem)

- `Kernel/FileSystem/FileSystem.cs`: a tiny in-memory file system (directories + files) with basic operations.
- `Kernel/FileSystem/Console.cs`: parses commands and executes them (the command line interface).

### GUI (AlkaOS.GUI)

This folder is basically “the stuff you see on screen”:

- `GUI/ProcessVisualizer.cs`: shows process states (READY, RUNNING, TERMINATED).
- `GUI/MLFQVisualizer.cs`: visualizes the Multi-Level Feedback Queue scheduler and how processes move between queues.
- `GUI/MemoryVisualizer.cs`: draws memory frames and shows allocation + fragmentation.

### Utils

Glue scripts that connect the Godot UI to kernel actions:

- `Utils/AddProcess.cs`, `Utils/TerminateProcess.cs`, `Utils/SwitchToNextProcess.cs`: connect buttons to kernel actions.
- `Utils/RunConcurrencyDemo.cs`: old version of the demo (logic moved into `ConcurrencyDemo.cs`). Not used in the finished project.
- `Utils/WriteProcessInfo.cs`: old debug printing helper. Not used in the finished project.

## Operating system concepts used

### Process management and scheduling

I used a PCB structure to track each process. Scheduling is implemented behind a C# interface, and the simulation supports both Round Robin and MLFQ. Processes can be added, switched, and terminated dynamically, and the visualizers update in real time.

### Memory management

Memory is simulated using paging. Each process gets pages allocated and freed, and the memory visualizer shows how frames are shared and how fragmentation builds up.

### Concurrency

I implemented basic threading support using custom locks and condition variables. There’s a working readers–writers demo, and you can follow what’s happening from the logs.

This was the part I struggled with the most. At one point, if a race-condition-like situation happened, the app would freeze and memory usage would go crazy (I saw it spike up to around 30 GB). After a lot of debugging I realized the real problem was simpler: my backend is still single-threaded. If I make a “simulated thread” wait in a blocking way, I end up blocking the actual main thread that runs the whole simulation.

The fix was to switch the demo flow to a non-blocking approach (basically a “try and retry later” step), so the main loop keeps running even when a simulated thread is waiting for a lock. That stopped the freezing and the memory leak.

### File system

The file system is simple, but it supports creating, writing, and deleting files. Internally it’s just an in-memory tree (implemented via `DirectoryEntry` and `FileEntry`). Each directory stores its subdirectories and files, and navigation works by changing the “current directory” reference.

The console uses Godot’s `RichTextLabel`, which is also why I reused that same output area for the concurrency demo logs. The file system doesn’t persist anything between runs, but it still feels like a small shell and covers the basic operations.

## Running the project

- Open the project in [Godot Editor](https://godotengine.org/download/archive/4.5.2-stable/) and press Play.
- Alternatively, a precompiled Windows binary is included in the [releases](https://github.com/atelieralice/AlkaOS/releases) section.

Optional build check (requires .NET 8 SDK):

```sh
dotnet build
```

## File system console commands

- `pwd`, `ls`, `cd <dir>` (also `cd ..`, `cd /`)
- `mkdir <dir>`, `touch <file>`, `rm <name>`
- `cat <file>`, `write <file> <content>`

## Credits / third-party assets

This project includes some third-party assets used for an educational student project. All copyrights belong to their respective owners.

- Background image: steam_bg.png
  - Source: [素材屋あいりす様](https://sozaiyairis.com/)
- Project icon: flask.png

## Disclaimer

AlkaOS is a student project for a 3rd-year Operating Systems course. It is not affiliated with or endorsed by any third-party brands.
