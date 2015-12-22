using UnityEngine;
using System.Collections;

[System.Serializable]
public class Waypoint {
	public bool walkable;
	public Vector3 position;
	public GameObject model;
	public Waypoint[] neighbours;
	public Trigger[] triggers;

	WaypointCollider modelCollider;

	// Pathfinding
	public Waypoint parent;
	public float gCost;
	public float hCost;
	public float fCost {
		get {
			return gCost + hCost;
		}
	}

	public Waypoint(bool _walkable, Vector3 _position, Waypoint[] _neighbours, Trigger[] _triggers) {
		walkable = _walkable;
		position = _position;
		neighbours = _neighbours;
		triggers = _triggers;

		SetModel ();
	}

	void SetModel () {
		model = GameObject.Instantiate (MapController.waypointCollider);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = position;

		modelCollider = model.GetComponent<WaypointCollider> ();
		modelCollider.waypoint = this;

		if (!walkable) {
			modelCollider.enabled = false;
		}

	}

//	public void Click() {
//		foreach (var _trigger in triggers) {
//			foreach (var _waypoint in _trigger.activateWaypoints) {
//				_waypoint.walkable = !_waypoint.walkable;
//			}
//		}
//	}

	public void Activate() {
		walkable = !walkable;
		modelCollider.enabled = walkable;
	}

	public void Trigger() {
		foreach (var _trigger in triggers) {
			_trigger.Activate ();
		}
	}
}
