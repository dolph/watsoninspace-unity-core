using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookingAtDisplay : MonoBehaviour {

	public Text lookingatText;

	// Update is called once per frame
	void Update () {
		lookingatText.text = "LookingAt: " + LookingAtTracker.activeObject;
	}
}
