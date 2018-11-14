using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialVelocity : MonoBehaviour {

	public float Velocity = 0.5f;

	// Use this for initialization
	void Start () {
		float x = Velocity * (Random.value - 0.5f);
		float y = Velocity * (Random.value - 0.5f);
		float z = Velocity * (Random.value - 0.5f);
		Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
		rb.velocity = new Vector3 (x, y, z);
	}

	// Update is called once per frame
	void Update () {

	}
}
