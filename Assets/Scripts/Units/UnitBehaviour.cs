using UnityEngine;
using System.Collections.Generic;

public enum UnitRotation {
	Forward = 0,
	ForwardRight,
	BackwardRight,
	Backward,
	BackwardLeft,
	ForwardLeft
}

public static class UnitBehaviour {
	public static Unit player;

	public static void GoTo (Unit _unit, Waypoint _target) {
		List<Waypoint> path = MapController.FindPath (_unit.path[0], _target);
		if (path != null) {
			_unit.path = path;
			MakeStep (_unit);
		}
	}

	public static void MakeStep(Unit _unit) {
		if (_unit.path == null) {
			Debug.LogWarning ("Path == null");
			_unit.model.GetComponent<Move>().enabled = false;
			_unit.model.GetComponent<Rotate>().enabled = false;
			return;
		}

		if (_unit.path.Count < 2) {
			return;
		}
		_unit.path.RemoveAt (0);

		_unit.model.GetComponent<Rotate> ().target = _unit.path[0].position;
		_unit.model.GetComponent<Rotate> ().enabled = true;

		_unit.model.GetComponent<Move>().target = _unit.path[0].position;
		_unit.model.GetComponent<Move>().enabled = true;

		// Activate triggers
		foreach (var _trigger in _unit.path [0].triggers) {
			_trigger.Activate ();
		}
	}
}
