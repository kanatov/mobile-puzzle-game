using UnityEngine;
using System.Collections.Generic;

public class DynamicObject {
	public GameObject model;
	public string prefab;
	public List<Waypoint> path;
	public int currentWaypoint;
}