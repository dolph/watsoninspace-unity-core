// source: youtube.com/watch?v=blO039OzUZc

using UnityEngine;
using System.Collections;

public class MouseLookCtrl : MonoBehaviour {

	Vector2 mouseLook;
	Vector2 smoothV;
	public float sensitivity = 5.0f;
	public float smoothing = 2.0f;

	GameObject character;
	Rigidbody characterRigidBody;

	// Use this for initialization
	void Start () {
		character = this.transform.parent.gameObject; // character is the camera's parent
		characterRigidBody = character.GetComponent<Rigidbody> (); // character is the camera's parent
	}

	// Update is called once per frame
	void Update () {
		var input = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y")); // mouse delta

		input = (sensitivity * smoothing) * input;
		smoothV.x = Mathf.Lerp (smoothV.x, input.x, 1f / smoothing); // Lerp = linear interpolation - creates smooth motion
		smoothV.y = Mathf.Lerp (smoothV.y, input.y, 1f / smoothing); // Lerp = linear interpolation - creates smooth motion
		mouseLook += smoothV; // sums the total deltas in order to apply to character

		var rotation = Quaternion.identity;
		rotation.eulerAngles = new Vector3 (-mouseLook.y, mouseLook.x, 0);
		character.transform.rotation = rotation;
		characterRigidBody.rotation = rotation;
	}
}