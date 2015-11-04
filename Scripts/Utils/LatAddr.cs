using UnityEngine;
using System;


// This class describes a Lattice Address
// A Lattice Address is a linear combination of the basis vectors of a specific lattice
// In this case, the basis vectors are the offsets from the barycenter of 2-Simplex
public class LatAddr {

	/* ******************
	 * 	STATIC MEMBERS
	 * ******************/

	public static LatAddr convertVectorToLatAddr (Vector2 inVec) {
		LatAddr result = null;
		float[,] mat = BasisVectors.getBasisVectorMatrix(1f);
		mat[0,2] = inVec.x;
		mat[1,2] = inVec.y;

		LinearEquationSolver.Solve(mat);

		result = new LatAddr 
			((int) Mathf.Round (mat[0,2]),
			 (int) Mathf.Round (mat[1,2]),
			 (0));

		return result;
	}

	public static Vector2 convertLatAddrToVector (LatAddr lAddr) {
		Vector2 result = new Vector2();

		result += BasisVectors.getBasisVector(0) * lAddr.a;
		result += BasisVectors.getBasisVector(1) * lAddr.b;
		result += BasisVectors.getBasisVector(2) * lAddr.c;

		return result;
	}

	/* ******************
	 * 	INSTANCE MEMBERS
	 * ******************/

	private int a, b, c;

	public int A
	{
		get { return this.a; }

		set { this.a = value; }
	}

	public int B
	{
		get { return this.b; }
		
		set { this.b = value; }
	}

	public int C
	{
		get { return this.c; }
		
		set { this.c = value; }
	}

	public LatAddr () {
		this.a = 0;
		this.b = 0;
		this.c = 0;
	}

	// Shallow Copy Constructor
	public LatAddr (LatAddr src) {
		this.a = src.a;
		this.b = src.b;
		this.c = src.c;

		this.cleanUpLatAddr();
	}
	
	public LatAddr (int a, int b, int c) {
		this.a = a;
		this.b = b;
		this.c = c;
		
		this.cleanUpLatAddr();
	}

	public void cleanUpLatAddr ()  {
		int min = this.a;
		min = (min < this.b) ? (min) : (this.b);
		min = (min < this.c) ? (min) : (this.c);

		this.a -= min;
		this.b -= min;
		this.c -= min;
	}

	public void addLatAddr (LatAddr op) {
		this.a += op.a;
		this.b += op.b;
		this.c += op.c;

		this.cleanUpLatAddr();
	}
}
