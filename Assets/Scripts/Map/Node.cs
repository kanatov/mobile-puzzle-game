using UnityEngine;
using System.Collections;

[System.Serializable]
public class Node {
	
	[System.Serializable]
	public class NeighboursReferenceIndexer {
		[SerializeField] int[] walkNodes;

		public Node this [int i] {
			get {
				return MapController.walkNodes [walkNodes [i]];
			}
			set {
				walkNodes [i] = value.id;
			}
		}

		public int Length {
			get {
				return walkNodes.Length;
			}
		}

		public NeighboursReferenceIndexer (int[] _neighbours) {
			walkNodes = _neighbours;
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
	public NodeTypes Type {
		get {
			return (NodeTypes)type;
		}
		set {
			type = (int)value;
		}
	}

	[SerializeField] bool walk;
	public bool Walk {
		get {
			return walk;
		}
		set {
			walk = value;
			collider.enabled = value;
		}
	}

	public int id;
	public bool activated;
	public bool activateOnTouch;
	public bool noRepeat;

	public NeighboursReferenceIndexer WalkNodes;
	public TriggersReferenceIndexer Triggers;

	[System.NonSerialized] public GameObject model;
	[System.NonSerialized] NodeCollider modelCollider;
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
	[System.NonSerialized] public Node parent;
	[System.NonSerialized] public float gCost;
	[System.NonSerialized] public float hCost;
	public float fCost {
		get {
			return gCost + hCost;
		}
	}

	// Constructor
	public Node(int _id, NodeTypes _type, bool _enabled, bool _noRepeat, bool _activateOnTouch, Vector3 _position, int[] _neighbours, int[] _triggers) {
		id = _id;
		Type = _type;
		noRepeat = _noRepeat;
		activateOnTouch = _activateOnTouch;
		Position = _position;
		WalkNodes = new NeighboursReferenceIndexer(_neighbours);
		Triggers = new TriggersReferenceIndexer(_triggers);

		SetModel (_enabled);
	}

	public void SetModel (bool _enabled) {
		model = GameObject.Instantiate (MapController.nodeCollider);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = Position;

		modelCollider = model.GetComponent<NodeCollider> ();
		modelCollider.node = this;

		collider = model.GetComponent<SphereCollider>();

		Walk = _enabled;
		activated = false;
	}

	bool SetWaypointAsActivatedOnce () {
		if (noRepeat && activated) {
			return true;
		}
		activated = !activated;
		return false;
	}

	public void ActivateWalk() {
		if (SetWaypointAsActivatedOnce ()) {
			return;
		}

		Walk = !Walk;
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
