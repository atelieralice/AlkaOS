using System;
using System.Collections.Generic;
using Godot;

namespace AlkaOS.Kernel;

public partial class MemoryManager : Node // Now inherits Node for signals
{
	[Signal]
	public delegate void MemoryChangedEventHandler ( );

	private int frameCount;
	private int frameSize;
	private bool[] frames; // true = used, false = free
	public int[] FrameOwners { get; private set; } // -1 = free, otherwise process ID

	public MemoryManager ( int frameCount = 256, int frameSize = 4096 ) {
		this.frameCount = frameCount;
		this.frameSize = frameSize;
		frames = new bool[frameCount];
		FrameOwners = new int[frameCount];
		for ( int i = 0; i < frameCount; i++ )
			FrameOwners[i] = -1;
	}

	// Allocates n pages for a process, returns a page table
	public Dictionary<int, int> AllocatePages ( int numPages, int processId = -1 ) {
		var pageTable = new Dictionary<int, int> ( );
		int allocated = 0;
		for ( int i = 0; i < frames.Length && allocated < numPages; i++ ) {
			if ( !frames[i] ) {
				frames[i] = true;
				pageTable[allocated] = i; // virtual page -> physical frame
				FrameOwners[i] = processId;
				allocated++;
			}
		}
		if ( allocated < numPages )
			throw new Exception ( "Not enough memory!" );
		EmitSignal ( SignalName.MemoryChanged );
		return pageTable;
	}

	// Free all frames in a page table
	public void FreePages ( Dictionary<int, int> pageTable ) {
		foreach ( var frame in pageTable.Values ) {
			frames[frame] = false;
			FrameOwners[frame] = -1;
		}
		EmitSignal ( SignalName.MemoryChanged );
	}

	// Translate a virtual address to a physical address
	public int TranslateAddress ( Dictionary<int, int> pageTable, int virtualAddress ) {
		int page = virtualAddress / frameSize;
		int offset = virtualAddress % frameSize;
		if ( !pageTable.ContainsKey ( page ) )
			throw new Exception ( "Invalid page!" );
		int frame = pageTable[page];
		return frame * frameSize + offset;
	}
}