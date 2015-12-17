using UnityEngine;
using System.Collections;

[System.Serializable]
public class Waypoint {
	public bool walkable;
	public Vector3 position;
	public GameObject model;
	public Waypoint[] neighbours;
	public UnitRotation[] rotations;
	public Trigger[] triggers;

	// Pathfinding
	public Waypoint parent;
	public float gCost;
	public float hCost;
	public float fCost {
		get {
			return gCost + hCost;
		}
	}

	public Waypoint(bool _walkable, Vector3 _position, Waypoint[] _neighbours, UnitRotation[] _rotations, Trigger[] _triggers) {
		walkable = _walkable;
		position = _position;
		neighbours = _neighbours;
		rotations = _rotations;
		triggers = _triggers;

		SetModel ();
	}

	void SetModel () {
		model = GameObject.Instantiate (MapController.waypointCollider);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = position;

		WaypointCollider modelCollider = model.GetComponent<WaypointCollider> ();
		modelCollider.waypoint = this;
	}

	public void Click() {
		foreach (var _trigger in triggers) {
			foreach (var _waypoint in _trigger.waypoints) {
				_waypoint.walkable = !_waypoint.walkable;
			}
		}
	}
}
