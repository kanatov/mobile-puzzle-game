using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointDT : MonoBehaviour {
	// Cell properties
	public List<GameObject> triggers;
	public List<GameObject> neighbours;
	public bool walkable;

	// Unit properties
	public UnitsTypes unitType = UnitsTypes.None;
	public UnitRotation unitRotation;
	public GameObject[] unitWaypoints;
}