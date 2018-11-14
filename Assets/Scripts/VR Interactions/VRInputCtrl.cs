using UnityEngine;
using System.Collections;

public class VRInputCtrl : MonoBehaviour {

	private Animator GloveAnimator;

	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device Controller {
		get { return SteamVR_Controller.Input((int) trackedObject.index);}
	}

	private bool isAwake = false;
	private float pulseTimer = 0f;
	private ushort pulseStrength = 0;

	void Awake() {
		isAwake = true;
		trackedObject = GetComponent<SteamVR_TrackedObject>();

		GloveAnimator = GetComponentInChildren<Animator>();
	}

	// Update is called once per frame
	void Update () {

		// There is an active haptic pulse to trigger
		if (this.pulseTimer > 0f) {
			this.pulseTimer -= Time.deltaTime;
			this.TriggerHapticPulse(this.pulseStrength);
		}

		if (Controller.GetAxis() != Vector2.zero) {
			// Debug.Log(gameObject.name + Controller.GetAxis());
		}

		if (Controller.GetHairTriggerDown()) {
			// Debug.Log(gameObject.name + " Trigger Down");
		}

		if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
			// Debug.Log(gameObject.name + " Trigger Press");
		}

		if (Controller.GetHairTriggerUp()) {
			// Debug.Log(gameObject.name + " Trigger Release");
		}

		if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
			// Debug.Log(gameObject.name + " Touchpad Press");
		}

		if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
			// Debug.Log(gameObject.name + " Grip Press Down");
			GloveAnimator.SetBool("GripHand", true);
		}

		if (Controller.GetPress(SteamVR_Controller.ButtonMask.Grip)) {
			// Debug.Log(gameObject.name + " Grip Press");
		}

		if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
			// Debug.Log(gameObject.name + " Grip Release");
			GloveAnimator.SetBool("GripHand", false);
		}
	}

	/*
		Input Controls
	*/
	private bool isDeviceReady() {
		return isAwake && (int) trackedObject.index >= 0;
	}

	public float GetHorizontalAxis () {
		return isDeviceReady() ? Controller.GetAxis().x : 0f;
	}

	public float GetVerticalAxis () {
		return isDeviceReady() ? Controller.GetAxis().y : 0f;
	}

	public bool GetPadDown () {
		return isDeviceReady() && Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
	}

	public bool GetPadUp () {
		return isDeviceReady() && Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad);
	}

	public bool isTriggerDown () {
		return isDeviceReady() && Controller.GetHairTriggerDown();
	}

	public bool isTriggerPress () {
		return isDeviceReady() && Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger);
	}

	public bool isTriggerUp () {
		return isDeviceReady() && Controller.GetHairTriggerUp();
	}

	public bool isGripDown () {
		return isDeviceReady() && Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip);
	}

	public bool isGripUp () {
		return isDeviceReady() && Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip);
	}

	/*
		Feedback
	*/

	public void TriggerHapticFeedback(ushort strength, float duration) {
		pulseStrength = strength;
		pulseTimer = duration;
	}

	public void TriggerHapticPulse(ushort strength) {
		Controller.TriggerHapticPulse(strength);
	}

	/*
		Physics
	*/

	public Vector3 GetVelocity () {
		return Controller.velocity;
	}

	public Vector3 GetAngularVelocity () {
		return Controller.angularVelocity;
	}
}
