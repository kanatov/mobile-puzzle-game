using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Unit : DynamicObject {
	public UnitRotation rotation;

	public Unit (string _prefab, UnitRotation _rotation, Waypoint _source) {
		prefab = _prefab;
		rotation = _rotation;

		Path = new PathIndexer (new List<Waypoint>{ _source });

		SetModel ();
	}

	public void SetModel () {
		if (prefab.Contains("Friend")) {
			MapController.player = this;
		}

		model = GameObject.Instantiate (Resources.Load<GameObject>(prefab));

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = Path[0].Position;
		modelTransform.eulerAngles = MapController.GetEulerAngle (rotation);

		model.GetComponent<Move> ().target = Path[0].Position;
		model.GetComponent<Move> ().unit = this;
		model.GetComponent<Rotate> ().target = Path[0].Position;
	}

	public void GoTo (Waypoint _target) {
		List<Waypoint> newPath = MapController.GetPath (Path[0], _target);
		if (newPath != null) {
			Path = new PathIndexer (newPath);
			Move ();
		}
	}

	public void Move() {
		if (Path == null) {
			Debug.LogWarning ("Path == null");
			model.GetComponent<Move>().enabled = false;
			model.GetComponent<Rotate>().enabled = false;
			return;
		}

		if (Path.Count < 2) {
			GameController.SaveGameSession ();
			return;
		}
		Path.RemoveAt (0);

		model.GetComponent<Rotate> ().target = Path[0].Position;
		model.GetComponent<Rotate> ().enabled = true;

		model.GetComponent<Move>().target = Path[0].Position;
		model.GetComponent<Move>().enabled = true;

		// Activate triggers
		for (int i = 0; i < Path [0].Triggers.Length; i++) {
			Path [0].Triggers[i].Activate ();
		}
	}
}
