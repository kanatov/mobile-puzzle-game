using UnityEngine;
using System.Collections;

[System.Serializable]
public class Node
{
	
	[SerializeField] bool walk;

	public bool Walk {
		get {
			return walk;
		}
		set {
			walk = value;
			if (nodeCollider != null) {
				nodeCollider.GetComponent<SphereCollider> ().enabled = value;
			}
		}
	}

	public int id;
	public bool activated;
	public bool activateOnTouch;
	public bool noRepeat;

	public NodeTypes type;
	public Direction ladderDirection;

	public NodeIndexer LocalNodes;
	public NodeIndexer WalkNodes;
	public TriggersIndexer Triggers;

	// Reference to hit collider controller
	[System.NonSerialized] NodeCollider nodeCollider;
	
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
	public Node
	(
		int _id,
		NodeTypes _type,
		bool _walk,
		bool _noRepeat,
		bool _activateOnTouch,
		Vector3 _position,
		int[] _walknodes,
		int[] _localnodes,
		int[] _triggers
	)
	{
		id = _id;
		type = _type;
		Walk = _walk;
		noRepeat = _noRepeat;
		activateOnTouch = _activateOnTouch;
		Position = _position;
		WalkNodes = new NodeIndexer (_walknodes);
		LocalNodes = new NodeIndexer (_localnodes);
		Triggers = new TriggersIndexer (_triggers);

		SetCollider ();
	}

	public void SetCollider ()
	{
		GameObject colliderObject = GameObject.Instantiate (MapController.nodeCollider);

		Transform modelTransform = colliderObject.GetComponent<Transform> ();
		modelTransform.localPosition = Position;

		nodeCollider = colliderObject.GetComponent<NodeCollider> ();
		nodeCollider.node = this;
		nodeCollider.GetComponent<SphereCollider> ().enabled = Walk;
	}

	bool SetWaypointAsActivatedOnce ()
	{
		if (noRepeat && activated) {
			return true;
		}
		activated = !activated;
		return false;
	}

	public void ActivateWalk ()
	{
		if (SetWaypointAsActivatedOnce ()) {
			return;
		}

		Walk = !Walk;
	}

	public void ActivateTriggers ()
	{
		if (SetWaypointAsActivatedOnce ()) {
			return;
		}

		for (int i = 0; i < Triggers.Length; i++) {
			Triggers [i].Activate ();
		}
	}
}
