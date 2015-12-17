using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointDT : MonoBehaviour {
	public bool walkable;

	public UnitsTypes unitType = UnitsTypes.None;
	public UnitRotation unitRotation;
	public GameObject[] unitWaypoints;

	public List<GameObject> triggers;
	public List<GameObject> neighbours;
	public List<UnitRotation> rotations;
}
