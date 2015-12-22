using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Trigger : DynamicObject{
	public Waypoint[] activateWaypoints;
	public bool activated;
	public bool noRepeat;

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
			model.GetComponent<Transform>().position = path[currentWaypoint].position;
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
