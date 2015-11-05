using UnityEngine;
using System.Collections;

public abstract class TreeObj {

	protected TreeObj prnt;
	protected CanAddr addr;
	protected int aggLev;

	public CanAddr Address
	{
		get { return this.addr; }
	}

	public TreeObj Parent
	{
		get { return this.prnt; }
	}

	public int AggregateLevel
	{
		get { return this.aggLev; }
	}

	public abstract Tile getTile (CanAddr cAddr);

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
	public abstract void inheritState (byte state);
	public abstract void setState (CanAddr cAddr, byte state);
}
