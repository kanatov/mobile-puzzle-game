using UnityEngine;
using System.Collections;

[System.Serializable]
public class Trigger {
	public Vector3[] coordinates;
	public GameObject model;
	public string prefab;
	public Waypoint[] waypoints;
	public bool activated;

	public void Launch() {
		ActivateWaypoints ();
	}

	public void ActivateWaypoints () {
		foreach (var _waypoint in waypoints) {
			_waypoint.walkable = !_waypoint.walkable;
		}
	}
}
