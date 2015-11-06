using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

// This script handles the necessary simulation functions and threading behaviors
public class Simulation : MonoBehaviour {
	
	public static readonly float timeStep = 0.3f;
	
	// TODO: Some other event should ignite the starting tile and set this to be true
	public static volatile bool simRunning = true;
	private static readonly AutoResetEvent simSync = new AutoResetEvent(false);

	private static HashSet <StateChangeEvent> [] visDBuf = new HashSet<StateChangeEvent> [2];
	private static HashSet <StateChangeEvent> [] simDBuf = new HashSet<StateChangeEvent> [2];

	// Both the UI and Worker threads will be writing to the HashSet indicated by this index
	// and will be reading from the other HashSet.
	// This index is switched during the synchronizeSimulation Coroutine
	private static volatile int bufIndex = 0;

	// NOTE: The following three fields are used by the worker thread to query the terrain data.
	// The terrain data cannot be accessed by other threads, so necessary information has been copied.
	private static float[,] terrainHeighMap;

	public static int terrainHeight;
	public static int terrainWidth;

	private static float simToTerScale = 3.0f;
	private static float terToSimScale;

	private Thread simThread = null;

	void Start () {
		terToSimScale = 1 / simToTerScale;

		terrainHeighMap = Terrain.activeTerrain.terrainData.GetHeights (0, 0, Terrain.activeTerrain.terrainData.heightmapWidth, Terrain.activeTerrain.terrainData.heightmapHeight);
		terrainHeight = Terrain.activeTerrain.terrainData.heightmapHeight;
		terrainWidth = Terrain.activeTerrain.terrainData.heightmapWidth;

		visDBuf [0] = new HashSet <StateChangeEvent> ();
		visDBuf [1] = new HashSet <StateChangeEvent> ();

		simDBuf [0] = new HashSet <StateChangeEvent> ();
		simDBuf [1] = new HashSet <StateChangeEvent> ();

		simThread = new Thread(Simulation.runTimeStep);
		simThread.Start();

		SpaceTree.getTile(new CanAddr()).curTemp = 110;

		StartCoroutine (synchronizeSimulation());
	}
	
	IEnumerator synchronizeSimulation () {
		while (simRunning) {
			bufIndex = (bufIndex + 1) % 2;
			simDBuf[bufIndex].Clear();

			simSync.Set();

			if (visDBuf [(bufIndex+1) %2].Count == 0) {
				yield return new WaitForSeconds (timeStep);
			} else {
				float visChangeStep = timeStep / visDBuf [(bufIndex+1) % 2].Count;
				
				foreach (StateChangeEvent vbEvent in visDBuf [(bufIndex+1) % 2]) {
					// TODO: This code is temporary, just for visualization

					if (vbEvent.state == Tile.FIRE_MASK) {
						Vector2 tilePos = convertSimToTerrain(LatAddr.convertLatAddrToVector(CanAddr.convertCanAddrToLatAddr(vbEvent.tileAddr)));
						float [,,] map = new float[4,4,3];

						for (int i = 0; i < 4; i++) {
							for (int j = 0; j < 4; j++) {
								map[i, j, 0] = 0;
								map[i, j, 1] = 1;
								map[i, j, 2] = 0;
							}
						}

						Terrain.activeTerrain.terrainData.SetAlphamaps(Mathf.RoundToInt(tilePos.x) - 2, Mathf.RoundToInt(tilePos.y) - 2, map);

					}
					else {
						Vector2 tilePos = convertSimToTerrain(LatAddr.convertLatAddrToVector(CanAddr.convertCanAddrToLatAddr(vbEvent.tileAddr)));
						float [,,] map = new float[4,4,3];
						
						for (int i = 0; i < 4; i++) {
							for (int j = 0; j < 4; j++) {
								map[i, j, 0] = 0;
								map[i, j, 1] = 0;
								map[i, j, 2] = 1;
							}
						}
						
						Terrain.activeTerrain.terrainData.SetAlphamaps(Mathf.RoundToInt(tilePos.x) - 2, Mathf.RoundToInt(tilePos.y) - 2, map);
					}
					
					yield return new WaitForSeconds (visChangeStep);
				}
			}
		}
	}

	private static void runTimeStep () {
		while (simRunning) {
			simSync.WaitOne();
			visDBuf[bufIndex].Clear();

			// TODO: Apply all SimBoundEvents from simDBuf[(bufIndex+1) %2]
			// TODO: Run one time step of the simulation
			SpaceTree.updateState();
			SpaceTree.updateProperties();
		}
	}

	// NOTE: This function expects incoming position to be in "Terrain Coordinates"
	// TODO: If I care & have time, have the returned height value be interpolated between integer values
	public static float getHeightForPos (Vector2 pos) {
		int posX = Mathf.RoundToInt(pos.x);
		int posY = Mathf.RoundToInt(pos.y);
		return terrainHeighMap[posX, posY];
	}

	public static Vector2 convertSimToTerrain (Vector2 simCoords) {
		Vector2 result = new Vector2 (terrainWidth/2, terrainHeight/2);
		result += (simCoords * simToTerScale);
		return result;
	}

	public static Vector2 convertTerrainToSim (Vector2 terCoords) {
		Vector2 result = new Vector2(terCoords.x - (terrainWidth/2), terCoords.y - (terrainHeight/2));
		result *= terToSimScale;
		return result;
	}

	public static void postVisBoundChange (StateChangeEvent scEvnt) {
		visDBuf[bufIndex].Add(scEvnt);
	}
}