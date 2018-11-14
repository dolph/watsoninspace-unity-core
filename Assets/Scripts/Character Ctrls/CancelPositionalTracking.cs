using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelPositionalTracking : MonoBehaviour {

	public SteamVR_Camera TrackedObject;
	private Vector3 startPos;

	// Use this for initialization
	void Start () {
		startPos = this.gameObject.transform.localPosition;
	}

	// Update is called once per frame
	void Update () {
		Vector3 head = TrackedObject.gameObject.transform.localPosition;
		this.gameObject.transform.localPosition = startPos - TrackedObject.transform.localPosition;
		// TrackedObject.transform.localPosition = new Vector3(0f, 0f, 0f);
	}
}
