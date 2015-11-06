using UnityEngine;
using System.Threading;
using System.Collections;

public class Wind : MonoBehaviour {

	public static object windLock = new object();

	public static Vector2 WindVec = new Vector2();

	private static int windChangeStepsPerSec = 10;

	private static float windChangeRateMin = 0.5f;
	private static float windChangeRateMax = 2.0f;

	private static float windMagnitudeMin = 0.3f;
	private static float windMagnitudeMax = 3.0f;

	private static float windChangeTimerMin = 5.0f;
	private static float windChangeTimerMax = 20.0f;

	void Start () {
		StartCoroutine(changeWind());
	}

	IEnumerator changeWind () {
		while (Simulation.simRunning) {
			float randomAngle = Random.Range(0, 2*Mathf.PI);
			Vector2 newWindVec = new Vector2 (Mathf.Sin(randomAngle), Mathf.Cos(randomAngle));
			newWindVec *= Random.Range(windMagnitudeMin, windMagnitudeMax);

			Vector2 oldWindVec = new Vector2 (WindVec.x, WindVec.y);

			float windChangeRate = Random.Range(windChangeRateMin, windChangeRateMax);
			int windChangeSteps = Mathf.RoundToInt (windChangeStepsPerSec * windChangeRate);
			float windChangeIncrement = 1.0f/windChangeSteps;


			for (int i = 0; i < windChangeSteps; i++) {
				while (!Monitor.TryEnter(windLock)) {
					yield return new WaitForFixedUpdate();
				}
				WindVec = Vector2.Lerp(oldWindVec, newWindVec, i * windChangeIncrement);
				Monitor.Exit(windLock);
				yield return new WaitForSeconds (windChangeIncrement * windChangeRate);
			}

			yield return new WaitForSeconds(Random.Range(windChangeTimerMin, windChangeTimerMax));
		}
	}
}
