using UnityEngine;
using System.Collections;

[System.Serializable]
public class Waypoint {
	public bool walkable;
	public Trigger[] triggers;
	public Waypoint[] neighbours;

	[System.NonSerialized] public GameObject model;

	[System.NonSerialized] WaypointCollider modelCollider;
	[System.NonSerialized] SphereCollider modelSphereCollider;

	[SerializeField]float x;
	[SerializeField]float y;
	[SerializeField]float z;

	public Vector3 position {
		get {
			return new Vector3 (x, y, z);
		}
		set {
			x = value.x;
			y = value.y;
			z = value.z;
		}
	}

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

	public void SetModel () {
		model = GameObject.Instantiate (MapController.waypointCollider);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = position;

		modelCollider = model.GetComponent<WaypointCollider> ();
		modelCollider.waypoint = this;

		modelSphereCollider = model.GetComponent<SphereCollider>();
		modelSphereCollider.enabled = walkable;
	}

	public void Activate() {
		walkable = !walkable;
		modelSphereCollider.enabled = walkable;
	}

	public void Trigger() {
		foreach (var _trigger in triggers) {
			_trigger.Activate ();
		}
	}
}
