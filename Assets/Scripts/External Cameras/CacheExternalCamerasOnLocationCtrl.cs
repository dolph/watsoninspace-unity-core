using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheExternalCamerasOnLocationCtrl : MonoBehaviour {

	// when the user is in htese locations, the cameras will be active
	public LocationTracker [] ActiveLocations;
	public Camera[] CameraFeeds;

	private RenderTexture[] _screenTextures;
	// are the cameras/screens currently cached?
	private bool[] _cached;

	private bool _active = true;

	// Use this for initialization
	void Start () {
		// get the associated screen textures for each camera
		_screenTextures = new RenderTexture[CameraFeeds.Length];
		// default no cameras start cached
		_cached = new bool[CameraFeeds.Length];
		int i = 0;
		foreach (Camera c in CameraFeeds) {
			_screenTextures[i] = c.targetTexture;
			_cached[i] = false;
			i++;
		}
	}

	// Update is called once per frame
	void Update () {
		bool activateCameras = false;
		for (int i = 0; i < ActiveLocations.Length; i++) {
			// is the user in a nearby location?
			if (LocationTracker.currentLocation == ActiveLocations[i].locationName) {
				activateCameras = true;
			}
		}
		if (activateCameras && !_active) {
			// user is in a location that triggers the cameras to turn on
			UncacheCameras();
		} else if (!activateCameras && _active) {
			CacheCameras();
		}
	}

	private void CacheCameras () {
		_active = false;
		foreach (Camera c in CameraFeeds) {
			Debug.Log("Disable: ");
			Debug.Log(c);
			c.gameObject.SetActive(false);
		}
	}

	private void UncacheCameras () {
		_active = true;
		foreach (Camera c in CameraFeeds) {
			Debug.Log("Enable: ");
			Debug.Log(c);
			c.gameObject.SetActive(true);
		}
	}
}
