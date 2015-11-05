using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Tile t = SpaceTree.getTile(new CanAddr());
		t.curTemp = 300;

		//SpaceTree.setState(new CanAddr(), (byte) 0x02);

		CanAddr cAddr = new CanAddr();
		cAddr.setTuple (3, 3);

		//SpaceTree.setState(cAddr, 0x04);

		this.StartCoroutine(SimUpdate(0.1f));
	}
	
	void FixedUpdate () {

	}

	IEnumerator SimUpdate (float waitTime) {
		while (true) {
			//Debug.Log("Running time step");
			SpaceTree.updateState();
			SpaceTree.updateProperties();
			yield return new WaitForSeconds (waitTime);
		}
	}
}
