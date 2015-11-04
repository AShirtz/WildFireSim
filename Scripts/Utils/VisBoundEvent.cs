using UnityEngine;
using System.Collections;

// This class represents any event that the Simulation produces that should affect the visualization.
// As this project has a near target, this class has been designed to only represent tile state change.
public class VisBoundEvent {
	public byte state;
	public CanAddr tileAddr;
}
