using UnityEngine;
using System;


// This class describes a Conanical Address, which is taken from the paper
// "An isomorphism between the p-adic integers and a ring associated with a tiling of N-space by permutohedra"
// This class assumes Order-3, or 2 dimensional tiling
public class CanAddr {

	/* ******************
	 * 	STATIC MEMBERS
	 * ******************/

	private static byte TUPLE_MASK = 0x07;

	public static CanAddr convertLatAddrToCanAddr (LatAddr lAddr) {
		CanAddr result = new CanAddr();
		LatAddr tmp = new LatAddr(lAddr);

		byte curTup = 0x00;
		int i = 0;

		while ((tmp.A != tmp.B || tmp.A != tmp.C) && (i < Config.TREE_DEPTH)) {
			curTup = (byte) ((tmp.A + (2 * tmp.B) + (4 * tmp.C)) % 7);
			curTup &= TUPLE_MASK;

			result.setTuple(curTup, i);

			tmp.A = tmp.A - ((curTup >> 0) & 0x01);
			tmp.B = tmp.B - ((curTup >> 1) & 0x01);
			tmp.C = tmp.C - ((curTup >> 2) & 0x01);

			int a = tmp.A;
			int b = tmp.B;
			int c = tmp.C;

			tmp.A = ((4*a)+b+(2*c))/7;
			tmp.B = ((2*a)+(4*b)+c)/7;
			tmp.C = (a+(2*b)+(4*c))/7;

			i++;
		}

		return result;
	}


	// TODO: ordering check
	public static LatAddr convertCanAddrToLatAddr (CanAddr cAddr) {
		LatAddr result = new LatAddr();
		LatAddr tmp = new LatAddr();

		for (int i = 0; i < Config.TREE_DEPTH; i++) {
			byte curTup = cAddr.getTuple(i);
			tmp.A = (curTup >> 0) & 0x01;
			tmp.B = (curTup >> 1) & 0x01;
			tmp.C = (curTup >> 2) & 0x01;

			for (int j = i; j > 0; j--) {
				BasisVectors.applyBMatrixToLatAddr(tmp);
			}

			result.A += tmp.A;
			result.B += tmp.B;
			result.C += tmp.C;
		}

		result.cleanUpLatAddr();
		return result;
	}

	/* ******************
	 * 	INSTANCE MEMBERS
	 * ******************/

	private UInt32 val;

	public CanAddr () {
		this.val = 0;
	}

	// Shallow Copy Constructor
	public CanAddr (CanAddr cAddr) {
		this.val = cAddr.val;
	}

	// NOTE: The bytes go from most to least significant
	//  getTuple (Config.TREE_DEPTH - 1) is the most significant, getTuple(0) is least significant
	public byte getTuple (int index) {
		return (byte) ((this.val >> (3*index)) & TUPLE_MASK);
	}

	public void setTuple (byte val, int index) {
		this.val = (UInt32) this.val & ~(((UInt32) TUPLE_MASK) << (3*index));
		this.val = (UInt32) this.val | (((UInt32)(val & TUPLE_MASK)) << (3*index));
	}

	public String ToString () {
		String result = "";

		for (int i = Config.TREE_DEPTH - 1; i >= 0; i--) {
			result += this.getTuple(i) + ", ";
		}

		return result;
	}
}
