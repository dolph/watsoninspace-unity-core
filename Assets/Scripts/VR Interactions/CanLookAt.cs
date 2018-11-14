using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanLookAt : MonoBehaviour {

	public Collider LookingAtCollider;
	public Renderer HighlightObject;
	private Material HighlightMaterial;

	private bool _lookingAt = false;

	// Use this for initialization
	void Start () {
		if (LookingAtCollider == null) {
			// if not overriden in the Unity Editor
			LookingAtCollider = this.gameObject.GetComponent<Collider>();
		}
		if (HighlightObject == null) {
			// if not overriden in the Unity Editor
			HighlightObject = this.gameObject.GetComponent<Renderer>();
		}
		HighlightMaterial = Resources.Load<Material>("Materials/LookAtHighlight");
	}

	// Update is called once per frame
	void Update () {
		if (!_lookingAt && (LookingAtTracker.activeObject == LookingAtCollider.transform.name)) {
			if (HighlightObject != null) {
				ApplyOutline();
				_lookingAt = true;
			}
		} else if (_lookingAt && (LookingAtTracker.activeObject != LookingAtCollider.transform.name)) {
			ClearOutline();
			_lookingAt = false;
		}
	}

	private void ApplyOutline() {
		int MaterialCount = HighlightObject.materials.Length;
		Material[] materials = new Material[MaterialCount + 1];
		for (int i = 0; i < MaterialCount; i++) {
			materials[i] = HighlightObject.materials[i];
		}
		materials[MaterialCount] = HighlightMaterial;
		HighlightObject.materials = materials;
	}

	private void ClearOutline() {
		int MaterialCount = HighlightObject.materials.Length;
		Material[] materials = new Material[MaterialCount - 1];
		for (int i = 0; i < (MaterialCount - 1); i++) {
			materials[i] = HighlightObject.materials[i];
		}
		HighlightObject.materials = materials;
	}
}
