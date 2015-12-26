using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointDT : MonoBehaviour {
	// Cell properties
	public List<GameObject> triggers;
	public List<GameObject> neighbours;
	public bool walkable;
	public int type;
	public WaypointsTypes Type {
		get {
			return (WaypointsTypes)type;
		}
		set {
			type = (int)value;
		}
	}
	public Direction ladderDirection;

	// Unit properties
	public GameObject unitModel;
	public string unitPrefab;
	public Direction unitDirection;
}