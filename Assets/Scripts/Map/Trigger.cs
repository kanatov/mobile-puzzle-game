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

		public Waypoint this [int i] {
			get {
				return MapController.waypoints [activateWaypoints [i]];
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
		List<Waypoint> _path,
		string _prefab,
		Direction _tileDirection,
		int _currentWaypoint,
		int[] _activateWaypoints,
		bool _removeOnActivation
	) {
		id = _id;
		prefab = _prefab;
		modelRotation = _tileDirection;
		Path = new PathIndexer (_path);
		ActivateWaypoints = new ActivateWaypointsIndexer (_activateWaypoints);
		currentWaypoint = _currentWaypoint;
		removeOnActivation = _removeOnActivation;

		SetModel ();
	}

	public void SetModel () {
		if (prefab == "") {
			return;
		}

		model = GameObject.Instantiate (Resources.Load<GameObject> (prefab));
		model.GetComponent<Transform> ().position = Path [PositionInPath].Position;
		model.GetComponent<Transform> ().eulerAngles = MapController.GetEulerAngle(modelRotation);
		model.tag = "Trigger";
	}

	public void Activate() {
		if (removeOnActivation) {
			GameObject.Destroy (model.gameObject);
			return;
		}

		Move ();
		for (int i = 0; i < ActivateWaypoints.Length; i++) {
			ActivateWaypoints [i].ActivateWalkable ();
		}
	}

	public void Move () {
		if (model == null) {
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
