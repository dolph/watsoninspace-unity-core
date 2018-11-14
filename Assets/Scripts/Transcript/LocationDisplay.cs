using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationDisplay : MonoBehaviour {

	public Text locationText;

	// Update is called once per frame
	void Update () {
		locationText.text = "Location: " + LocationTracker.currentLocation;
	}
}
