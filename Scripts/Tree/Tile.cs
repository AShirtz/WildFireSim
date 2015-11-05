using UnityEngine;
using System.Collections;

public class Tile : TreeObj {

	/* ******************
	 * 	STATIC MEMBERS
	 * ******************/

	public static readonly byte FIRE_MASK 				= 0x01;
	public static readonly byte BURN_MASK 				= 0x02;
	public static readonly byte WETTED_MASK 			= 0x04;
	public static readonly byte INCOMBUSTIBLE_MASK		= 0x08;
	public static readonly byte OUTSIDE_TERRAIN_MASK	= 0x16;

	public static int NUM_NEIGHBORS = 12;
	
	private static LatAddr getNeighborLatOffset (int nIndex) {
		switch (nIndex) {
		case 0:
			return new LatAddr(1,0,0);
		case 1:
			return new LatAddr(0,1,0);
		case 2:
			return new LatAddr(1,1,0);
		case 3:
			return new LatAddr(0,0,1);
		case 4:
			return new LatAddr(1,0,1);
		case 5:
			return new LatAddr(0,1,1);
		case 6:
			return new LatAddr(2,1,0);
		case 7:
			return new LatAddr(1,2,0);
		case 8:
			return new LatAddr(0,2,1);
		case 9:
			return new LatAddr(0,1,2);
		case 10:
			return new LatAddr(1,0,2);
		case 11:
			return new LatAddr(2,0,1);
		default:
			Debug.LogError ("Invalid Neighbor Index");
			return null;
		}
	}

	/* ******************
	 * 	TREEOBJ MEMBERS
	 * ******************/



	public override Tile getTile (CanAddr cAddr) {
		return this;
	}


	// TODO: The contents of these two methods are temporary, and should be replaced when you have a better understanding of the fire simulation
	public override void updateState () {
		byte prevStatus = this.state;

		if (!this.isOnFire()) {
			if (this.curTemp >= 100) { this.igniteTile(); }
		} else {
			if (this.curFuel <= 0) { this.extinguishTile(); }
		}

		if (prevStatus != this.state) {
			this.prnt.notifyStateChange(this.addr.getTuple(this.aggLev), this.state);
			Simulation.postVisBoundChange(new StateChangeEvent(new CanAddr(this.addr), this.state));
		}
	}

	public override void updateProperties () {
		if (this.isOnFire()) {
			this.curFuel -= 10;
			for (int i = 0; i < NUM_NEIGHBORS; i++) {
				if (this.outRef[i] != null) { 
					if (i < 6) 	{ this.outRef[i].curTemp += 13; }
					else 		{ this.outRef[i].curTemp += 2; }
				} 
			}
		} else {
			this.curTemp = (this.curTemp <= 0) ? (0) : (this.curTemp - 0);
		}
	}

	public override void inheritState (byte state) {
		this.state = state;
	}

	public override void setState (CanAddr cAddr, byte state) {
		this.state = state;
		this.prnt.notifyStateChange(this.addr.getTuple(this.aggLev), this.state);
	}

	// Both of these methods should be NO-OPS.
	// Tiles won't have children who report state change to them.
	public override void notifyStateChange (int childIndex, byte childStatus) {}
	public override void removalRequest (int childIndex) {}
	
	public void notifyReferenceAddition (int index) {
		this.inRef = (ushort) (this.inRef | (0x01 << index));
	}

	public void notifyReferenceRemoval (int index) {
		this.inRef = (ushort) (this.inRef & ~(0x01 << index));

		if (this.inRef == 0 && !this.isOnFire()) { this.prnt.removalRequest(this.addr.getTuple(this.aggLev)); }
	}


	/* ******************
	 * 	TILE SPECIFIC
	 * ******************/
	
	Tile[] outRef	= new Tile[NUM_NEIGHBORS];
	ushort inRef = 0x0000;

	private float elevation;

	private byte state = 0x00;
	
	// The below are placeholders until the ignition PDE is better understood
	public float curTemp = 0f;
	float curFuel = 100f;

	public Tile (TreeObj prnt, int nextTuple) {
		this.prnt = prnt;
		this.addr = new CanAddr(prnt.Address);
		this.aggLev = prnt.AggregateLevel - 1;
		this.addr.setTuple((byte) nextTuple, this.aggLev);
		this.elevation = Simulation.getHeightForPos(Simulation.convertSimToTerrain(LatAddr.convertLatAddrToVector(CanAddr.convertCanAddrToLatAddr(this.addr))));
	}

	public bool isOnFire () {
		return (this.state & Tile.FIRE_MASK) != 0;
	}

	public bool isBurned () {
		return (this.state & Tile.BURN_MASK) != 0;
	}

	public bool isWetted () {
		return (this.state & Tile.WETTED_MASK) != 0;
	}

	public bool isInCombustible () {
		return (this.state & Tile.INCOMBUSTIBLE_MASK) != 0;
	}

	public bool isOutsideTerrain () {
		return (this.state & Tile.OUTSIDE_TERRAIN_MASK) != 0;
	}

	// This method should not be called directly
	// Instead, change the properties to be the desired values and then call updateState
	public void igniteTile () {
		//Debug.Log("IgniteTile " + this.addr.ToString());
		// Guard for already burned tiles
		if (this.isBurned()) { return; }

		// Set state to include onFire
		this.state = (byte) (this.state | Tile.FIRE_MASK);
		LatAddr lAddr = CanAddr.convertCanAddrToLatAddr(this.addr);
		for (int i = 0; i < NUM_NEIGHBORS; i++) {
			LatAddr nAddr = new LatAddr(lAddr);
			nAddr.addLatAddr(Tile.getNeighborLatOffset(i));
			this.outRef[i] = SpaceTree.getTile(CanAddr.convertLatAddrToCanAddr(nAddr));
			this.outRef[i].notifyReferenceAddition(i);
		}
	}

	// This method should not be called directly
	// Instead, change the properties to be the desired values and then call updateState
	public void extinguishTile () {
		//Debug.Log("ExtinguishTile " + this.addr.ToString());
		// Guard for tiles not on fire
		if (!this.isOnFire()) { return; }

		// Set state to exclude onFire
		this.state = (byte) (this.state & ~Tile.FIRE_MASK);
		// Set state to include isBurned
		this.state = (byte) (this.state | Tile.BURN_MASK);

		this.curTemp = 0;

		for (int i = 0; i < NUM_NEIGHBORS; i++) {
			if (this.outRef[i] != null) { this.outRef[i].notifyReferenceRemoval(i); }
			this.outRef[i] = null;
		}
	}
}
