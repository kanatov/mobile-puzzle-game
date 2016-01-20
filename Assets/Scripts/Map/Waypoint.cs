using UnityEngine;
using System.Collections;

[System.Serializable]
public class Waypoint {
	
	[System.Serializable]
	public class NeighboursReferenceIndexer {
		[SerializeField] int[] neighbours;

		public Waypoint this [int i] {
			get {
				return MapController.waypoints [neighbours [i]];
			}
			set {
				neighbours [i] = value.id;
			}
		}

		public int Length {
			get {
				return neighbours.Length;
			}
		}

		public NeighboursReferenceIndexer (int[] _neighbours) {
			neighbours = _neighbours;
		}
	}

	[System.Serializable]
	public class TriggersReferenceIndexer {
		[SerializeField] int[] triggers;

		public TriggersReferenceIndexer (int[] _triggers) {
			triggers = _triggers;
		}

		public Trigger this [int i] {
			get {
				return MapController.triggers [triggers [i]];
			}
			set {
				triggers [i] = value.id;
			}
		}

		public int Length {
			get {
				return triggers.Length;
			}
		}
	}

	public int type;
	public WaypointsTypes Type {
		get {
			return (WaypointsTypes)type;
		}
		set {
			type = (int)value;
		}
	}

	[SerializeField] bool colliderEnabled;
	public bool ColliderEnabled {
		get {
			return colliderEnabled;
		}
		set {
			colliderEnabled = value;
			collider.enabled = value;
		}
	}

	public int id;
	public bool activated;
	public bool activateOnTouch;
	public bool noRepeat;

	public NeighboursReferenceIndexer Neighbours;
	public TriggersReferenceIndexer Triggers;

	[System.NonSerialized] public GameObject model;
	[System.NonSerialized] WaypointCollider modelCollider;
	[System.NonSerialized] SphereCollider collider;

	[SerializeField]float x;
	[SerializeField]float y;
	[SerializeField]float z;

	public Vector3 Position {
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
	[System.NonSerialized] public Waypoint parent;
	[System.NonSerialized] public float gCost;
	[System.NonSerialized] public float hCost;
	public float fCost {
		get {
			return gCost + hCost;
		}
	}

	// Constructor
	public Waypoint(int _id, WaypointsTypes _type, bool _enabled, bool _noRepeat, bool _activateOnTouch, Vector3 _position, int[] _neighbours, int[] _triggers) {
		id = _id;
		Type = _type;
		noRepeat = _noRepeat;
		activateOnTouch = _activateOnTouch;
		Position = _position;
		Neighbours = new NeighboursReferenceIndexer(_neighbours);
		Triggers = new TriggersReferenceIndexer(_triggers);

		SetModel (_enabled);
	}

	public void SetModel (bool _enabled) {
		model = GameObject.Instantiate (MapController.waypointCollider);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = Position;

		modelCollider = model.GetComponent<WaypointCollider> ();
		modelCollider.waypoint = this;

		collider = model.GetComponent<SphereCollider>();

		ColliderEnabled = _enabled;
		activated = false;
	}

	bool SetWaypointAsActivatedOnce () {
		if (noRepeat && activated) {
			return true;
		}
		activated = !activated;
		return false;
	}

	public void ActivateWalkable() {
		if (SetWaypointAsActivatedOnce ()) {
			return;
		}

		ColliderEnabled = !ColliderEnabled;
	}

	public void ActivateTriggers() {
		if (SetWaypointAsActivatedOnce ()) {
			return;
		}

		for (int i = 0; i < Triggers.Length; i++) {
			Triggers [i].Activate ();
		}
	}
}
