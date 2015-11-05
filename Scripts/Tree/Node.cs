using UnityEngine;
using System.Collections;

public class Node : TreeObj {

	/* ******************
	 * 	STATIC MEMBERS
	 * ******************/

	public static int NUM_CHILDREN = 7;

	/* ******************
	 * 	TREEOBJ MEMBERS
	 * ******************/
	
	public override Tile getTile (CanAddr cAddr) {
		int childIndex = (int) cAddr.getTuple(this.aggLev - 1);

		if (children[childIndex] == null) {
			if (this.aggLev == 1) {
				children[childIndex] = new Tile (this, childIndex);
			}
			else { children[childIndex] = new Node(this, childIndex); }
			this.children[childIndex].inheritState(this.childState[childIndex]);
		}
		return children[childIndex].getTile(cAddr);
	}

	public override void removalRequest (int childIndex) {
		this.children[childIndex] = null;

		bool livingChildren = false;
		for (int i = 0; i < NUM_CHILDREN; i++) 		{ livingChildren = livingChildren | (this.children[i] != null); }

		if (!livingChildren && this.homogeneousChildrenCheck() && this.prnt != null) { this.prnt.removalRequest(this.addr.getTuple(this.aggLev)); }
	}

	// NOTE: I don't need to update my state here, as I will be notified of any child state change
	public override void updateState () {
		for (int i = 0; i < NUM_CHILDREN; i++) {
			if (children[i] != null) { children[i].updateState(); }
		}
	}

	public override void updateProperties () {
		for (int i = 0; i < NUM_CHILDREN; i++) {
			if (children[i] != null) { children[i].updateProperties(); }
		}
	}
	
	public override void notifyStateChange (int childIndex, byte childState) {
		this.childState[childIndex] = childState;
		if (this.homogeneousChildrenCheck() && this.prnt != null) { this.prnt.notifyStateChange(this.addr.getTuple(this.aggLev), this.childState[0]); }
	}

	public override void inheritState (byte state) {
		for (int i = 0; i < NUM_CHILDREN; i++) { this.childState[i] = state; }
	}

	public override void setState (CanAddr cAddr, byte state) {
		int childIndex = (int) cAddr.getTuple(this.aggLev - 1);

		if (children[childIndex] == null) {
			if (this.aggLev == 1) {
				this.childState[childIndex] = state;
			}
			else {
				this.children[childIndex] = new Node (this, childIndex);
				this.children[childIndex].inheritState (this.childState[childIndex]);
				this.children[childIndex].setState(cAddr, state);
			}
		}
		else {
			children[childIndex].setState(cAddr, state);
		}
	}

	/* ******************
	 * 	NODE SPECIFIC
	 * ******************/

	private TreeObj[] children = new TreeObj[NUM_CHILDREN];
	
	private byte[] childState = new byte[NUM_CHILDREN];
	
	public Node () {
		this.prnt = null;
		this.addr = new CanAddr();
		this.aggLev = Config.TREE_DEPTH;
	}
	
	public Node (TreeObj prnt, int nextTuple) {
		this.prnt = prnt;
		this.addr = new CanAddr(prnt.Address);
		this.aggLev = prnt.AggregateLevel - 1;
		this.addr.setTuple((byte) nextTuple, aggLev);
	}

	private bool homogeneousChildrenCheck () {
		byte cState = this.childState[0];
		bool homogeneousChildren = true;

		for (int i = 1; i < NUM_CHILDREN; i++)	{ homogeneousChildren = homogeneousChildren & (cState == this.childState[i]); }

		return homogeneousChildren;
	}
}
