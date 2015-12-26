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

	public bool activated;
	public bool noRepeat;
	public ActivateWaypointsIndexer ActivateWaypoints;

	public Trigger (List<Waypoint> _path, string _prefab, int _currentWaypoint, int[] _activateWaypoints) {
		prefab = _prefab;
		Path = new PathIndexer (_path);
		ActivateWaypoints = new ActivateWaypointsIndexer (_activateWaypoints);
		currentWaypoint = _currentWaypoint;

		SetModel ();
	}

	public void SetModel () {
		model = GameObject.Instantiate (Resources.Load<GameObject> (prefab));
		model.GetComponent<Transform> ().position = Path [PositionInPath].Position;
		model.tag = "Trigger";
	}

	public void Activate() {
		if (noRepeat && activated) {
			return;
		}
		
		activated = true;
		
		Move ();
		for (int i = 0; i < ActivateWaypoints.Length; i++) {
			ActivateWaypoints[i].ActivateWaypoints();
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
