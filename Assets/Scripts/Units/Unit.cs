using UnityEngine;
using System.Collections;

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
	}

	void SetModel () {
		model = GameObject.Instantiate (MapController.unitsModels[(int)type]);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = waypoints[0].position;
		modelTransform.eulerAngles = new Vector3 (0f, 60 * (int)rotation, 0f);
	}
}
