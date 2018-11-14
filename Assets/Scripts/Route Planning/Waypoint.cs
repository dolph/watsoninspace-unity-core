using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

	public static List<Waypoint> Waypoints = new List<Waypoint>();
	public Waypoint[] connectedWaypoints;

	// Use this for initialization
	void Start () {
		Waypoints.Add(this);
		// Check which module I'm in - location Tracker? Type Manually?
		foreach (Waypoint w in connectedWaypoints) {
			// if (!w.connectedWaypoints.contains(this)) {
			// 		w.connectedWaypoints.Add(this);
			// }
		}
	}

	// Update is called once per frame
	void Update () {

	}

	public Vector3 Location () {
		return this.transform.position;
	}
}
