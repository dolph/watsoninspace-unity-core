using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTracker : MonoBehaviour
{
    public string locationName; // What this section is, can be string, enum, int or whatever
    public static Dictionary<string, Vector3> moduleLocations = new Dictionary<string, Vector3>();

    public static string currentLocation; // The current section;

    void Awake() {
      moduleLocations.Add(locationName, this.transform.position);
    }

    void OnTriggerStay(Collider other)
    {
        if  (other.gameObject.name == "Player" && currentLocation != this.locationName) {
            // Could equally be a layer check or gameobject check
            currentLocation = this.locationName;
						Debug.Log("Player Moved To: " + locationName);
        }
    }
}
