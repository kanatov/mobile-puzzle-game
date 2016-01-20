using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Unit : DynamicObject {
	public Unit (string _prefab, Direction _rotation, Waypoint _source) {
		prefab = _prefab;
		modelRotation = _rotation;

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
		modelTransform.eulerAngles = MapController.GetEulerAngle (modelRotation);

		model.GetComponent<Move> ().Path = new List<Vector3> { Path [0].Position };
		model.GetComponent<Move> ().dynamicObject = this;
		model.GetComponent<Rotate> ().target = Path[0].Position;
	}

	public void GoTo (Waypoint _target) {
		List<Waypoint> newPath = MapController.GetPath (Path[0], _target);
		if (newPath != null) {
			Path = new PathIndexer (newPath);
			model.GetComponent<Move>().enabled = true;
		}
	}

	public override void Move() {
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

		List<Vector3> newPath = new List<Vector3> ();

		if (Path[0].Type == WaypointsTypes.Horisontal) {
			if (Path [1].Type == WaypointsTypes.Horisontal) {
				newPath.Add (Path [1].Position);
			}
			if (Path [1].Type == WaypointsTypes.Ladder) {
				newPath.Add (GetHorisontalSide (Path [0].Position, Path [1].Position));
				newPath.Add (GetLadderMiddle (Path [1].Position));
			}
		}

		// Set path
		if (Path [0].Type == WaypointsTypes.Ladder) {
			if (Path [1].Type == WaypointsTypes.Horisontal) {
				if (Path [0].Position.y > Path [1].Position.y) {
					newPath.Add (GetHorisontalSide (Path [0].Position, Path [1].Position));
				} else {
					newPath.Add (GetHorisontalSide (Path [1].Position, Path [0].Position));
				}
				newPath.Add (Path [1].Position);
			}

			if (Path [1].Type == WaypointsTypes.Ladder) {
				newPath.Add (GetLadderMiddle (Path [1].Position));
			}
		}

		model.GetComponent<Move> ().Path = newPath;
		model.GetComponent<Move>().enabled = true;
		model.GetComponent<Rotate> ().target = Path[1].Position;
		model.GetComponent<Rotate> ().enabled = true;

		// Activate triggers
		Path [1].ActivateTriggers ();
		Path.RemoveAt (0);
	}

	static Vector3 GetHorisontalSide (Vector3 _source, Vector3 _target) {
		_target.y = _source.y;
		Vector3 sidepoint;
		sidepoint = Vector3.Lerp(_source, _target, 0.5f);
		return sidepoint;
	}

	static Vector3 GetLadderMiddle (Vector3 _source) {
		_source.y += MapController.tileHeight / 2;
		return _source;
	}
}
