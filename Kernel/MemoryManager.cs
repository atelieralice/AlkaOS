using System;
using System.Collections.Generic;
using Godot;

namespace AlkaOS.Kernel;

public partial class MemoryManager : Node
{
	[Signal]
	public delegate void MemoryChangedEventHandler();

	private int frameCount;
	private int frameSize;
	private bool[] frames; // true = used, false = free
	public int[] FrameOwners { get; private set; } // -1 = free, otherwise process ID

	// Swapping and fragmentation
	private List<(int processId, int frame)> swapSpace = new List<(int, int)>();
	private HashSet<int> faultyFrames = new HashSet<int>();

	public MemoryManager(int frameCount = 256, int frameSize = 4096)
	{
		this.frameCount = frameCount;
		this.frameSize = frameSize;
		frames = new bool[frameCount];
		FrameOwners = new int[frameCount];
		for (int i = 0; i < frameCount; i++)
			FrameOwners[i] = -1;

		// Simulate faulty frames for fragmentation
		Random rng = new Random();
		while (faultyFrames.Count < 10)
			faultyFrames.Add(rng.Next(frameCount));
	}

	// Allocates n pages for a process, returns a page table
	public Dictionary<int, int> AllocatePages(int numPages, int processId = -1)
	{
		var pageTable = new Dictionary<int, int>();
		int allocated = 0;
		// Try to allocate from non-faulty and free frames
		for (int i = 0; i < frames.Length && allocated < numPages; i++)
		{
			if (!frames[i] && !faultyFrames.Contains(i))
			{
				frames[i] = true;
				pageTable[allocated] = i; // virtual page -> physical frame
				FrameOwners[i] = processId;
				allocated++;
			}
		}
		// If not enough, swap out frames (simulate swapping)
		while (allocated < numPages)
		{
			int victimFrame = FindVictimFrame();
			int victimOwner = FrameOwners[victimFrame];
			swapSpace.Add((victimOwner, victimFrame));
			frames[victimFrame] = false;
			FrameOwners[victimFrame] = -1;
			// Now allocate to this process
			frames[victimFrame] = true;
			pageTable[allocated] = victimFrame;
			FrameOwners[victimFrame] = processId;
			allocated++;
		}
		if (allocated < numPages)
			throw new Exception("Not enough memory!");
		EmitSignal(SignalName.MemoryChanged);
		return pageTable;
	}

	private int FindVictimFrame()
	{
		// Pick the first used non-faulty frame
		for (int i = 0; i < frames.Length; i++)
			if (frames[i] && !faultyFrames.Contains(i))
				return i;
		throw new Exception("No victim frame found!");
	}

	// Free all frames in a page table
	public void FreePages(Dictionary<int, int> pageTable)
	{
		foreach (var frame in pageTable.Values)
		{
			frames[frame] = false;
			FrameOwners[frame] = -1;
		}
		EmitSignal(SignalName.MemoryChanged);
	}

	// Translate a virtual address to a physical address
	public int TranslateAddress(Dictionary<int, int> pageTable, int virtualAddress)
	{
		int page = virtualAddress / frameSize;
		int offset = virtualAddress % frameSize;
		if (!pageTable.ContainsKey(page))
			throw new Exception("Invalid page!");
		int frame = pageTable[page];
		return frame * frameSize + offset;
	}

	// For visualization
	public HashSet<int> GetFaultyFrames() => faultyFrames;
	public HashSet<int> GetSwappedFrames()
	{
		var set = new HashSet<int>();
		foreach (var (_, frame) in swapSpace)
			set.Add(frame);
		return set;
	}
}