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
	
	public override Tile getTile (CanAddr cAddr, int aggLev) {
		int childIndex = (int) cAddr.getTuple(aggLev);

		if (children[childIndex] == null) {
			if (aggLev == 0) {
				children[childIndex] = new Tile (this, childIndex, aggLev);
			}
			else { children[childIndex] = new Node(this, childIndex, aggLev); }
			this.children[childIndex].acceptState(this.childState[childIndex]);
		}
		return children[childIndex].getTile(cAddr, aggLev-1);
	}

	public override void removalRequest (int childIndex) {
		this.children[childIndex] = null;

		bool livingChildren = false;
		for (int i = 0; i < NUM_CHILDREN; i++) 		{ livingChildren = livingChildren | (this.children[i] != null); }

		if (!livingChildren && this.homogeneousChildrenCheck() && this.prnt != null) { this.prnt.removalRequest(this.addr.getTuple(this.aggregateLevel)); }
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
		if (this.homogeneousChildrenCheck() && this.prnt != null) { this.prnt.notifyStateChange(this.addr.getTuple(this.aggregateLevel), this.childState[0]); }
	}

	public override void acceptState (byte state) {
		for (int i = 0; i < NUM_CHILDREN; i++) { this.childState[i] = state; }
	}

	/* ******************
	 * 	NODE SPECIFIC
	 * ******************/

	private TreeObj[] children = new TreeObj[NUM_CHILDREN];
	private int aggregateLevel;
	
	private byte[] childState = new byte[NUM_CHILDREN];
	
	public Node () {
		this.prnt = null;
		this.addr = new CanAddr();
	}
	
	public Node (Node prnt, int nextTuple, int aggLev) {
		this.prnt = prnt;
		this.addr = new CanAddr(prnt.Address);
		this.addr.setTuple((byte) nextTuple, aggLev);
		this.aggregateLevel = aggLev;
	}

	private bool homogeneousChildrenCheck () {
		byte cState = this.childState[0];
		bool homogeneousChildren = true;

		for (int i = 1; i < NUM_CHILDREN; i++)	{ homogeneousChildren = homogeneousChildren & (cState == this.childState[i]); }

		return homogeneousChildren;
	}
}
