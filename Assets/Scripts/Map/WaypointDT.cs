using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointDT : MonoBehaviour {
	// Cell properties
	public List<GameObject> triggers;
	public List<GameObject> neighbours;
	public bool walkable;
	public WaypointsTypes waypointType;
	public Direction ladderDirection;

	// Unit properties
	public GameObject unitModel;
	public string unitPrefab;
	public Direction unitDirection;
}