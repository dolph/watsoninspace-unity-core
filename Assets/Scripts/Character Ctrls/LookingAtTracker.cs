using System.Collections;
using UnityEngine;

public class LookingAtTracker : MonoBehaviour {

	public static string activeObject;

	Camera cam;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
	}

	// Update is called once per frame
	void Update () {
		// ignore the location trackers
		int layerMask = 1 << 11;
		layerMask = ~layerMask;

		Ray ray = cam.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
			activeObject = hit.transform.name;
		} else {
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.white);
			activeObject = "<no-object>";
		}

		// if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {
		// 	Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
		// 	activeObject = hit.collider.gameObject.name;
		// 	Debug.Log("Did Hit" + activeObject);
		// } else {
		// 	Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
		// 	Debug.Log("Did not hit");
		// }
	}
}
