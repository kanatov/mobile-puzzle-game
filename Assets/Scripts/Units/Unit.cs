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
			UnitBehaviour.player = this;
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
}
