using UnityEngine;
using System.Collections;

// This class is represents any event that the player makes that will affect the Simulation.
// As this project has a near target, this class has been designed to only represent tile state change.
public class SimBoundEvent {
	public byte state;
	public CanAddr tileAddr;
}
