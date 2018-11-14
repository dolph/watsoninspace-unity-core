using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheExternalCameraCtrl : MonoBehaviour {

	public Camera CameraFeed;

	private bool _cached;
	private RenderTexture ScreenTexture;

	// Use this for initialization
	void Start () {
		ScreenTexture = CameraFeed.targetTexture;
	}

	// Update is called once per frame
	void Update () {
		if (!_cached && ScreenTexture.IsCreated()) {
			CameraFeed.gameObject.SetActive(false);
			_cached = true;
		}
	}
}
