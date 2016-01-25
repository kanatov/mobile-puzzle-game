using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Trigger : DynamicObject{

	[System.Serializable]
	public class ActivateWaypointsIndexer {
		[SerializeField] int[] activateWaypoints;

		public ActivateWaypointsIndexer (int[] _activateWaypoints) {
			activateWaypoints = _activateWaypoints;
		}

		public Node this [int i] {
			get {
				return MapController.currentLevelNodes [activateWaypoints [i]];
			}
			set {
				activateWaypoints [i] = value.id;
			}
		}

		public int Length {
			get {
				return activateWaypoints.Length;
			}
		}
	}

	public ActivateWaypointsIndexer ActivateWaypoints;
	public bool removeOnActivation;

	public Trigger (
		int _id, 
		List<Node> _path,
		string _prefabPath,
		Direction _tileDirection,
		int _currentWaypoint,
		int[] _activateWaypoints,
		bool _removeOnActivation
	) {
		id = _id;
		prefabPath = _prefabPath;
		modelDirection = _tileDirection;
		Path = new PathIndexer (_path);
		ActivateWaypoints = new ActivateWaypointsIndexer (_activateWaypoints);
		currentNode = _currentWaypoint;
		removeOnActivation = _removeOnActivation;

		SetModel ();
	}

	public override void SetModel () {
		if (prefabPath == "") {
			return;
		}

		model = GameObject.Instantiate (Resources.Load<GameObject> (prefabPath));
		model.GetComponent<Transform> ().position = Path [PositionInPath].Position;
		model.GetComponent<Transform> ().eulerAngles = MapController.GetEulerAngle(modelDirection);
		model.tag = "Trigger";
	}

	public void Activate() {
		for (int i = 0; i < ActivateWaypoints.Length; i++) {
			ActivateWaypoints [i].ActivateWalk ();
		}

		if (removeOnActivation) {
			GameObject.Destroy (model.gameObject);
			return;
		}

		Move ("activate");
	}

	public override void Move (string _callback) {
		if (prefabPath == null) {
			return;
		}

		if (_callback == "move") {
			return;
		}

		Move modelMove = model.GetComponent<Move> ();
		if (modelMove == null) {
			modelMove = model.AddComponent<Move> ();
			modelMove.enabled = false;
			modelMove.dynamicObject = this;
		}

		if (Path.Count < 2) {
			return;
		}

		PositionInPath = PositionInPath + 1;

		modelMove.Path = new List<Vector3> { Path [PositionInPath].Position };
		modelMove.enabled = true;
	}
}
