using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DynamicObject {
	public string prefab;
	public List<Waypoint> path;
	public int currentWaypoint;

	[System.NonSerialized]
	public GameObject model;
}