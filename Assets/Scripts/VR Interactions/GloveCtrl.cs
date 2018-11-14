using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveCtrl : MonoBehaviour {

	public VRInputCtrl VRInput;

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
			if (VRInput.isGripDown()) {
				// Reset Motion
				Debug.Log("Grip Down");
				animator.SetBool("GripHand", true);
			}
			if (VRInput.isGripUp()) {
				// Reset Motion
				Debug.Log("Grip Up");
				animator.SetBool("GripHand", false);
			}
	}
}
