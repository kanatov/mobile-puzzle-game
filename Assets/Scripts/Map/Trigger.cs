using UnityEngine;
using System.Collections;

[System.Serializable]
public class Trigger {
	public TriggersTypes type;
	public Vector3 position;
	public int state;
	public Waypoint[] waypoints;
	public GameObject model;

	public Trigger(TriggersTypes _type, Vector3 _position, int _state, Waypoint[] _waypoints) {
		type = _type;
		position = _position;
		state = _state;
		waypoints = _waypoints;

		SetModel ();
	}

	void SetModel () {
		model = GameObject.Instantiate (MapController.triggersModels[(int)type]);

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = position;
	}
}
