using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class RoutePlannerCtrl : MonoBehaviour {

	public AssistantHandler WatsonAssistant;
	public LineRenderer line;

	private string _destination;
	private Dictionary<Waypoint, int> _calculatedWeights = new Dictionary<Waypoint, int>();

	// Use this for initialization
	void Start () {
		WatsonAssistant.AssistantResponded += OnAssistantResponded;
	}

	// Update is called once per frame
	void Update () {
		// use contains as e.g. "Node 2" in Assistant doesn't match "Node 2 - Harmony" in Unity
		if (_destination != null && LocationTracker.currentLocation.Contains(_destination)) {
			_destination = null;
			ClearPath();
		}
	}

	public void OnAssistantResponded(object source, OnAssistantResponseEventArgs e) {
		// Toggle the debug panel is the #action--debug intent recognised
		string module = ModuleRecognised(e.Entities);
		if (e.Intent != "" && e.Intent == "nav--path" && module != "") {
			ShowPathTo(module);
		}
	}

	private string ModuleRecognised(List<object> entities) {
		foreach (object e in entities) {
			Dictionary<string, object> eDict = (e as Dictionary<string, object>);
			string _entityType = eDict["entity"].ToString(); // _entity.ToString();
			if (_entityType == "ISS_Module") {
				return eDict["value"].ToString();
			}
		}
		return "";
	}

	private void ShowPathTo (string destination) {
		// store context of destination so we can remove the path once destination is reached
		_destination = destination;
		Vector3[] path = FindPathTo(destination);
		line.positionCount = path.Length;
		line.SetPositions(path);
	}

	private Vector3[] FindPathTo (string dest) {
		Waypoint nearest = FindNearestWaypoint();
		Waypoint destination = FindDestinationWaypoint(dest);
		_calculatedWeights = new Dictionary<Waypoint, int>();
		CalculateDistanceWeights(nearest, 0);
		List<Waypoint> waypoints = CalculateRoute(destination);
		return WaypointsToPath(waypoints);
	}

	private Waypoint FindNearestWaypoint () {
		Waypoint nearest = null;
		float nearestDistance = 1000f;
		foreach(Waypoint w in Waypoint.Waypoints) {
			float dist = Vector3.Distance(this.transform.position, w.Location());
			if (dist < nearestDistance) {
				nearestDistance = dist;
				nearest = w;
			}
		}
		return nearest;
	}

	private Waypoint FindDestinationWaypoint (string name) {
		foreach(Waypoint w in Waypoint.Waypoints) {
			if (w.name == name) {
				return w;
			}
		}
		return null;
	}

	// Given a <calculateWaypoint> calculate the distance to all nodes in the waypoint network
	private void CalculateDistanceWeights (Waypoint currentWaypoint, int currentLength) {
		if (currentWaypoint == null) {
			return;
		}
		bool continueCalculation = false;
		if (!_calculatedWeights.ContainsKey(currentWaypoint)) {
			// this waypoint has no weighting yet, i.e. it has not yet been visited in the weighting calculation
			// assign the current distance into the to this waypoint
			_calculatedWeights.Add(currentWaypoint, currentLength);
			// check further connections
			continueCalculation = true;
		} else if (currentLength < _calculatedWeights[currentWaypoint]) {
			// if we can get to the same waypoint quicker
			_calculatedWeights[currentWaypoint] = currentLength;
			// check further connections
			continueCalculation = true;
		}
		if (continueCalculation) {
			foreach(Waypoint w in currentWaypoint.connectedWaypoints) {
				CalculateDistanceWeights(w, currentLength + 1);
			}
		}
	}

	private List<Waypoint> CalculateRoute (Waypoint destination) {
		List<Waypoint> path = new List<Waypoint>();
		if (_calculatedWeights.ContainsKey(destination)) {
			Waypoint current = destination;
			for (int i = _calculatedWeights[destination]; i > 0; i--) {
				// insert at start of list
				path.Insert(0, current);
				// check connected waypoints
				foreach (Waypoint w in current.connectedWaypoints) {
					if (_calculatedWeights.ContainsKey(w) && _calculatedWeights[w] == (i - 1)) {
						// is next waypoint in the path
						current = w;
					}
				}
			}
			foreach (Waypoint w in path) {
				Debug.Log(w);
			}
		} else {
			// throw error
		}
		return path;
	}

	private Vector3[] WaypointsToPath (List<Waypoint> waypoints) {
		Vector3[] positions = new Vector3[waypoints.Count];
		int i = 0;
		Debug.Log("PATH");
		foreach (Waypoint w in waypoints) {
			Debug.Log(w);
			positions[i] = w.Location();
			i++;
		}
		return positions;
	}

	private void ClearPath () {
		line.positionCount = 0;
		line.SetPositions(new Vector3[0]);
	}

}
