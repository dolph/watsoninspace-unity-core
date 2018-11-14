using UnityEngine;
using System.Collections;

public class FirstPersonCtrl : MonoBehaviour {

	public float movementForce = 1.0f;
	public float maxSpeed = 3.0f;

	private Camera vrCamera;
	private Rigidbody rb;
	private CapsuleCollider col;

	public Transform Body;
	public VRInputCtrl LeftHand;
	public VRInputCtrl RightHand;

	// Use this for initialization
	void Start () {
		// prevent a cursor from rendering in the game & locks the cursor to ensure it stays inside the game window
		Cursor.lockState = CursorLockMode.Locked;

		rb = GetComponent<Rigidbody> ();
		col = GetComponent<CapsuleCollider> ();
		vrCamera = Camera.main;

    SteamVR_Fade.Start(Color.clear, 0f);
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("escape")) {
			Cursor.lockState = CursorLockMode.None;
		}

		if (Input.GetKey(KeyCode.R)) {
			// Reset Motion
			SlowDown();
		}

		if (LeftHand.isTriggerPress()) {
			// Reset Motion
			SlowDown();
		}
	}

	void FixedUpdate() {
		/* Physics Movement */
		// float moveHorizontal = Input.GetAxis ("Horizontal");
		// float moveVertical = Input.GetAxis ("Vertical");
		float moveHorizontal = LeftHand.GetHorizontalAxis();
		float moveVertical = LeftHand.GetVerticalAxis();

		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);
		movement = vrCamera.transform.TransformDirection(movement);
		rb.AddRelativeForce (movement * movementForce); // apply force relative to the RigidBody local axes

		this.transform.rotation = Quaternion.identity; // VRCameraRigEyes.rotation;

		Body.SetPositionAndRotation(vrCamera.transform.position, vrCamera.transform.rotation);
		rb.velocity = Vector3.ClampMagnitude (rb.velocity, maxSpeed); // make sure the Player does not move too fast
		// Debug.Log(rb.velocity);

		col.center = vrCamera.transform.localPosition;
	}

	private void SlowDown() {
		rb.velocity = rb.velocity * 0.7f;
		rb.angularVelocity = rb.angularVelocity * 0.7f;
		if (rb.velocity.magnitude > 0 && rb.velocity.magnitude < 0.3) {
			StopMotion ();
		}
		if (rb.angularVelocity.magnitude > 0) {
			StopRotation ();
		}
	}

	private void StopMotion() {
		rb.velocity = new Vector3 (0f, 0f, 0f);
	}

	private void StopRotation() {
		rb.velocity = new Vector3 (0f, 0f, 0f);
	}
}
