using UnityEngine;
using System.Collections;

public abstract class TreeObj {

	protected TreeObj prnt;
	protected CanAddr addr;

	public CanAddr Address
	{
		get { return this.addr; }
	}

	public TreeObj Parent
	{
		get { return this.prnt; }
	}

	public abstract Tile getTile (CanAddr cAddr, int depth);

	public abstract void removalRequest (int childIndex);

	public abstract void updateState ();
	public abstract void updateProperties ();
	
	// The newStatus byte contains the status of the Tile with the following format:
	//  [0]: onFire
	//  [1]: isBurned
	//  [2]: isWetted
	//  [3]: isInCombustible
	//  [4-7]: Currently Unused
	public abstract void notifyStateChange (int childIndex, byte childStatus);
	public abstract void acceptState (byte state);
}
