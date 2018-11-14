using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class CupolaActionHandler : MonoBehaviour {

	public AssistantHandler WatsonAssistant;
	public Animator CupolaWindowAnimator;

	private bool windowsOpen = false;

	// Use this for initialization
	void Start () {
		WatsonAssistant.AssistantResponded += CheckIntent;
	}

	void OnTriggerEnter(Collider other)
	{
		// If the user touches this button
		if  (other.gameObject.tag == "Grabber") {
			ToggleCupolaWindows();
			VibrateController(other.gameObject);
		}
	}

	private void CheckIntent (object source, OnAssistantResponseEventArgs e) {
		// if hte user uses their voice to request the windows to open/close
		if (e.Intent == "action--openclose" && LocationTracker.currentLocation == "Cupola") {
			ToggleCupolaWindows();
		}
	}

	private void ToggleCupolaWindows () {
		windowsOpen = !windowsOpen;
		CupolaWindowAnimator.SetBool("OpenCupola", windowsOpen);
	}

	private void VibrateController (GameObject grabber) {
		VRInputCtrl device = grabber.GetComponentInParent<VRInputCtrl>();
		device.TriggerHapticFeedback(2000, 0.2f);
	}
}
