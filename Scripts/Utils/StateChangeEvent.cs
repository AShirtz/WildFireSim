using UnityEngine;
using System.Collections;

// This class represents any change in state for a specific Tile.
// If the event affects multiple tiles, create multiple StateChangeEvents
public class StateChangeEvent {
	public byte state;
	public CanAddr tileAddr;

	public StateChangeEvent (CanAddr cAddr, byte state) {
		this.tileAddr = cAddr;
		this.state = state;
	}
}
