using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Unit : DynamicObject {
	public UnitRotation rotation;

	public Unit(string _prefab, UnitRotation _rotation, Waypoint _source) {
		prefab = _prefab;
		rotation = _rotation;

		path = new List<Waypoint> ();
		path.Add (_source);

		SetModel ();

		if (prefab.Contains("Friend")) {
			player = this;
		}
	}

	void SetModel () {
		model = GameObject.Instantiate (Resources.Load<GameObject>(prefab));

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = path[0].position;
		modelTransform.eulerAngles = MapController.GetEulerAngle (rotation);

		model.GetComponent<Move> ().target = path [0].position;
		model.GetComponent<Move> ().unit = this;
		model.GetComponent<Rotate> ().target = path [0].position;
	}

	public void GoTo (Waypoint _target) {
		List<Waypoint> newPath = MapController.GetPath (path[0], _target);
		if (newPath != null) {
			path = newPath;
			Move ();
		}
	}

	public void Move() {
		if (path == null) {
			Debug.LogWarning ("Path == null");
			model.GetComponent<Move>().enabled = false;
			model.GetComponent<Rotate>().enabled = false;
			return;
		}

		if (path.Count < 2) {
			return;
		}
		path.RemoveAt (0);

		model.GetComponent<Rotate> ().target = path[0].position;
		model.GetComponent<Rotate> ().enabled = true;

		model.GetComponent<Move>().target = path[0].position;
		model.GetComponent<Move>().enabled = true;

		// Activate triggers
		foreach (var _trigger in path [0].triggers) {
			_trigger.Activate ();
		}
	}
}
