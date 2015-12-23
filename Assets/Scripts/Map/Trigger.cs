using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Trigger : DynamicObject{
	public Waypoint[] activateWaypoints;
	public bool activated;
	public bool noRepeat;

	public Trigger (List<Waypoint> _path, string _prefab, int _currentWaypoint, Waypoint[] _activateWaypoints) {
		prefab = _prefab;
		path = _path;
		currentWaypoint = _currentWaypoint;
		activateWaypoints = _activateWaypoints;

		SetModel ();
	}

	public void SetModel () {
		model = GameObject.Instantiate (Resources.Load<GameObject> (prefab));
		model.GetComponent<Transform> ().position = path [currentWaypoint].position;
		model.tag = "Trigger";
	}

	public void Activate() {
		if (noRepeat && activated) {
			return;
		}
		
		activated = true;
		
		Move ();
		foreach (var _waypoint in activateWaypoints) {
			_waypoint.Activate();
		}
	}

	void Move () {
		if (model == null) {
			return;
		}

		Move modelMove = model.GetComponent<Move> ();
		if (modelMove == null) {
			modelMove = model.AddComponent<Move> ();
			modelMove.enabled = false;

			modelMove.target = path[currentWaypoint].position;
			modelMove.dynamicObject = this;
		}

		if (path.Count < 2) {
			return;
		}

		currentWaypoint++;
		if (currentWaypoint == path.Count) {
			currentWaypoint = 0;
		}

		modelMove.target = path[currentWaypoint].position;
		modelMove.enabled = true;
	}
}
