using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabCtrl : MonoBehaviour {

	private VRInputCtrl Hand;
	private float _distanceToGrabbingObject;
	private Collider _grabbingObject;
	private Transform _grabbingObjectOrigin;

	// Use this for initialization
	void Start () {
		Hand = this.gameObject.GetComponentInParent<VRInputCtrl>();
	}

	void OnTriggerStay(Collider other)
	{
		if (Hand.isGripDown() && other.tag == "Grabbable" && !other.gameObject.isStatic) {
			Debug.Log("Grip down, object is grabbable and not static");
			if (_grabbingObject == null) {
				// begin grabbing the object
				_grabbingObject = other;
				_grabbingObjectOrigin = _grabbingObject.transform.parent;
				_grabbingObject.transform.SetParent(this.transform);

				Rigidbody _grabbingObjectRB = _grabbingObject.GetComponent<Rigidbody>();
				_grabbingObjectRB.velocity = new Vector3();
				_grabbingObjectRB.angularVelocity = new Vector3();

				_distanceToGrabbingObject = Vector3.Distance(this.transform.position, _grabbingObject.transform.position);
			}
		}
		// if (Hand.isGripUp() && other.tag == "Grabbable") {
		// 	Debug.Log("[GRABCTRL] release:");
		// 	Debug.Log(other);
		// 	ReleaseObject();
		// }
	}

	// Update is called once per frame
	void Update () {
		if (Hand.isGripUp()) {
			ReleaseObject();
		}
		// if grabbing object is moving (i.e. has collided with something) - let it go
		if (_grabbingObject != null) {
			Debug.Log("User is holding...");
			// provide hand feedback to ditctae the user is holding the object
			Hand.TriggerHapticPulse(100);
			if (Mathf.Abs(Vector3.Distance(this.transform.position, _grabbingObject.transform.position) - _distanceToGrabbingObject) > 0.1f) {
				ReleaseObject();
			}
		}
	}

	private void ReleaseObject () {
		if (_grabbingObject != null) {
			_grabbingObject.transform.SetParent(_grabbingObjectOrigin);
			Rigidbody _grabbingObjectRB = _grabbingObject.GetComponent<Rigidbody>();
			_grabbingObjectRB.velocity = Hand.GetVelocity();
			_grabbingObjectRB.angularVelocity = Hand.GetAngularVelocity();

			_grabbingObject = null;
			_grabbingObjectOrigin = null;
		}
	}
}
