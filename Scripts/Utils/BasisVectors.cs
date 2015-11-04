using UnityEngine;
using System.Collections;

public class BasisVectors {
	
	private static Vector2[] basisVectors = 
	{
		new Vector2 ( 0f, 1f ),
		new Vector2 ( (Mathf.Sqrt(3f))/2f, -1f/2f ),
		new Vector2 ( -(Mathf.Sqrt(3f))/2f, -1f/2f )
	};

	public static float[,] getBasisVectorMatrix (float scale) {
		float[,] result = 
			{
				{basisVectors[0].x, basisVectors[1].x, 0f},
				{basisVectors[0].y, basisVectors[1].y, 0f}
			};
		return result;
	}

	public static Vector2 getBasisVector (int index) {
		return basisVectors[index];
	}

	public static void applyBMatrixToLatAddr (LatAddr lAddr) {
		int a = lAddr.A;
		int b = lAddr.B;
		int c = lAddr.C;

		lAddr.A = (2*a)-c;
		lAddr.B = (2*b)-a;
		lAddr.C = (2*c)-b;

		lAddr.cleanUpLatAddr();
	}
}
