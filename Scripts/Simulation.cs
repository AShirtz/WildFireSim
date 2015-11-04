using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

// This script handles the necessary simulation functions and threading behaviors
public class Simulation : MonoBehaviour {

	// TODO: What this class needs is:
	/*
	 * 1. Create Tree and both SimBoundEvent and VisBoundEvent double buffers		DONE
	 * 2. Create a worker thread and that runs one time step of the simulation then waits for the synchronization coroutine		DONE
	 * 3. Create Synchronization coroutine		DONE
	 */

	public static readonly float timeStep = 0.3f;
	
	// TODO: Some other event should ignite the starting tile and set this to be true
	private static volatile bool simRunning = true;
	private static readonly AutoResetEvent simSync = new AutoResetEvent(false);

	private static HashSet <VisBoundEvent> [] visDBuf = new HashSet<VisBoundEvent> [2];
	private static HashSet <SimBoundEvent> [] simDBuf = new HashSet<SimBoundEvent> [2];

	// Both the UI and Worker threads will be writing to the HashSet indicated by this index
	// and will be reading from the other HashSet.
	// This index is switched during the synchronizeSimulation Coroutine
	private static volatile int bufIndex = 0;

	private Thread simThread = null;
	
	void Start () {
		visDBuf [0] = new HashSet <VisBoundEvent> ();
		visDBuf [1] = new HashSet <VisBoundEvent> ();

		simDBuf [0] = new HashSet <SimBoundEvent> ();
		simDBuf [1] = new HashSet <SimBoundEvent> ();

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
				
				foreach (VisBoundEvent vbEvent in visDBuf [(bufIndex+1) % 2]) {
					// TODO: Make the required visual change
					
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
}