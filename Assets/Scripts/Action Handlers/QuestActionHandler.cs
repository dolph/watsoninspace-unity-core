using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class QuestActionHandler : MonoBehaviour {

	public AssistantHandler WatsonAssistant;
	public Animator QuestHatchAnimator;

	private bool windowsOpen = false;

	// Use this for initialization
	void Start () {
		WatsonAssistant.AssistantResponded += CheckIntent;
	}

	void OnTriggerEnter(Collider other)
	{
		// If the user touches this button
		if  (other.gameObject.tag == "Grabber") {
			ToggleQuestHatch();
			VibrateController(other.gameObject);
		}
	}

	private void CheckIntent (object source, OnAssistantResponseEventArgs e) {
		// if hte user uses their voice to request the windows to open/close
		if (e.Intent == "action--openclose" && (LocationTracker.currentLocation == "Quest" || LocationTracker.currentLocation == "Outside")) {
			ToggleQuestHatch();
		}
	}

	private void ToggleQuestHatch () {
		windowsOpen = !windowsOpen;
		QuestHatchAnimator.SetBool("OpenAirlock", windowsOpen);
	}

	private void VibrateController (GameObject grabber) {
		VRInputCtrl device = grabber.GetComponentInParent<VRInputCtrl>();
		device.TriggerHapticFeedback(1000, 0.1f);
	}
}
