using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Unit {
	public UnitsTypes type = UnitsTypes.None;
	public UnitRotation rotation;
	public Waypoint[] waypoints;
	public GameObject model;

	public Unit(UnitsTypes _type, UnitRotation _rotation, Waypoint[] _waypoints) {
		type = _type;
		rotation = _rotation;
		waypoints = _waypoints;

		SetModel ();

		if (type == UnitsTypes.Player) {
			UnitBehaviour.player = this;
		}
	}

	void SetModel () {
		model = GameObject.Instantiate (MapController.unitsModels[(int)type]);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = waypoints[0].position;
		modelTransform.eulerAngles = new Vector3 (
			0f,
			MapController.GetRotationDegree(rotation),
			0f
		);

		model.GetComponent<Move> ().source = waypoints [0];
		model.GetComponent<Rotate> ().target = waypoints [0];
	}

	public void GoTo (Waypoint _target) {
		model.GetComponent<Move>().path = MapController.FindPath (model.GetComponent<Move> ().source, _target);
		model.GetComponent<Move>().enabled = true;
	}
}
