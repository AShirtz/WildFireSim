using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Tile t = SpaceTree.getTile(new CanAddr());
		t.curTemp = 300;

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
