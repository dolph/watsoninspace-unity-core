using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class SolarArrayActionHandler : MonoBehaviour {

	public AssistantHandler WatsonAssistant;
	public Animator SolarArrayAnimator;

	private bool arraysRotated = false;

	// Use this for initialization
	void Start () {
		WatsonAssistant.AssistantResponded += CheckIntent;
	}

	private void CheckIntent (object source, OnAssistantResponseEventArgs e) {
		// if the user uses their voice to request the windows to open/close
		if (e.Intent == "action--rotate-arrays") {
			RotateArrays();
		}
	}

	private void RotateArrays () {
		arraysRotated = !arraysRotated;
		SolarArrayAnimator.SetBool("RotateArrays", arraysRotated);
	}
}
