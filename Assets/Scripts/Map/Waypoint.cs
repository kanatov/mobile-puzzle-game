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

	public int id;
	public bool walkable;
	public NeighboursReferenceIndexer Neighbours;
	public TriggersReferenceIndexer Triggers;

	[System.NonSerialized] public GameObject model;
	[System.NonSerialized] WaypointCollider modelCollider;
	[System.NonSerialized] SphereCollider modelSphereCollider;

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
	public Waypoint(int _id, bool _walkable, Vector3 _position, int[] _neighbours, int[] _triggers) {
		id = _id;
		walkable = _walkable;
		Position = _position;
		Neighbours = new NeighboursReferenceIndexer(_neighbours);
		Triggers = new TriggersReferenceIndexer(_triggers);

		SetModel ();
	}

	public void SetModel () {
		model = GameObject.Instantiate (MapController.waypointCollider);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = Position;

		modelCollider = model.GetComponent<WaypointCollider> ();
		modelCollider.waypoint = id;

		modelSphereCollider = model.GetComponent<SphereCollider>();
		modelSphereCollider.enabled = walkable;
	}

	public void ActivateWaypoints() {
		walkable = !walkable;
		modelSphereCollider.enabled = walkable;
	}

	public void ActivateTriggers() {
		for (int i = 0; i < Triggers.Length; i++) {
			Triggers [i].Activate ();
		}
	}
}
