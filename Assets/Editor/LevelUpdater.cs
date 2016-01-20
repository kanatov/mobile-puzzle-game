using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CustomEditor(typeof(MapControllerLoader))]
class LevelUpdater : Editor {
	// Waypoints
	[SerializeField] static GameObject[] waypoints;
	static float neighbourDistance = 0.9f;

	void OnSceneGUI() {
		DrawWaypointNetwork ();
	}

	public static void CreateWaypoints (GameObject _instanceObject) {
		GameObject[] items = Selection.gameObjects;

		foreach (var _item in items) {
			GameObject waypoint = GameObject.Instantiate (_instanceObject);
			waypoint.GetComponent<Transform> ().position = _item.GetComponent<Transform> ().position;
		}
	}

	// Place tiles to container
	public static void UpdateTiles () {
		MapController.SetContainer (MapController.TAG_TILE);
	}

	// Place waypoint to container
	// Update waypoint data
	public static void UpdateWaypoints () {
		waypoints = MapController.SetContainer (MapController.TAG_WAYPOINT);

		for (int a = 0; a < waypoints.Length; a++) {
			WaypointDT _waypointDT = waypoints[a].GetComponent<WaypointDT> ();
			SetNeighbours (_waypointDT);

			// Check triggers
			RemoveWrongTriggers (_waypointDT);
			RemoveEmpties (_waypointDT.triggers);

			// Set icon
			SetIcon (waypoints [a]);

			// Save changes
			EditorUtility.SetDirty(_waypointDT);
		}

		EditorSceneManager.MarkAllScenesDirty ();
	}

	static void SetNeighbours (WaypointDT _waypointDT) {
		Vector3 waypointDTPos = _waypointDT.GetComponent<Transform> ().position;
		_waypointDT.neighbours = new List<GameObject> ();

		_waypointDT.GetComponent<SphereCollider> ().enabled = false;
		Collider[] hitColliders = Physics.OverlapSphere(waypointDTPos, neighbourDistance);
		_waypointDT.GetComponent<SphereCollider> ().enabled = true;

		for (int i = 0; i < hitColliders.Length; i++) {
			WaypointDT neighbourDT = hitColliders [i].GetComponent<WaypointDT> ();
			Vector3 neighbourPos = hitColliders [i].GetComponent<Transform> ().position;

			// Hor
			if (_waypointDT.Type == WaypointsTypes.Horisontal) {
				// Hor to Hor
				if (neighbourDT.Type == WaypointsTypes.Horisontal) {
					if (waypointDTPos.y == neighbourPos.y) {
						_waypointDT.neighbours.Add (hitColliders [i].gameObject);
					}
				}

				// Hor to Ladder
				if (neighbourDT.Type == WaypointsTypes.Ladder) {
					if (waypointDTPos.y > neighbourPos.y) {
						if (CheckStraightConnection (_waypointDT, neighbourDT)) {
							_waypointDT.neighbours.Add (hitColliders [i].gameObject);
						}
					}
					if (waypointDTPos.y == neighbourPos.y) {
						if (CheckOppositConnection(_waypointDT, neighbourDT)) {
							_waypointDT.neighbours.Add (hitColliders [i].gameObject);
						}
					}
				}
			}

			// Ladder
			if (_waypointDT.Type == WaypointsTypes.Ladder) {

				// Ladder to Hor
				if (neighbourDT.Type == WaypointsTypes.Horisontal) {
					
					if (waypointDTPos.y < neighbourPos.y) {
						if (CheckStraightConnection (neighbourDT, _waypointDT)) {
							_waypointDT.neighbours.Add (hitColliders [i].gameObject);
						}
					}
					if (waypointDTPos.y == neighbourPos.y) {
						if (CheckOppositConnection(neighbourDT, _waypointDT)) {
							_waypointDT.neighbours.Add (hitColliders [i].gameObject);
						}
					}
				}

				// Ladder to Ladder
				if (neighbourDT.Type == WaypointsTypes.Ladder) {
					
					if (waypointDTPos.y > neighbourPos.y) {
						if (CheckStraightConnection (_waypointDT, neighbourDT)) {
							_waypointDT.neighbours.Add (hitColliders [i].gameObject);
						}
					}
					if (waypointDTPos.y < neighbourPos.y) {
						if (CheckOppositConnection (_waypointDT, neighbourDT)) {
							_waypointDT.neighbours.Add (hitColliders [i].gameObject);
						}
					}
				}
			}
		}
	}

	static bool CheckStraightConnection (WaypointDT _waypointDT, WaypointDT _neighbourDT) {
		Vector3 waypointDTPos = _waypointDT.GetComponent<Transform> ().position;
		WaypointDT neighbourDT = _neighbourDT.GetComponent<WaypointDT> ();
		Vector3 neighbourPos = _neighbourDT.GetComponent<Transform> ().position;

		if (neighbourDT.ladderDirection == MapController.GetPointDirection(waypointDTPos, neighbourPos)) {
			return true;
		}
		return false;
	}

	static bool CheckOppositConnection (WaypointDT _waypointDT, WaypointDT _neighbourDT) {
		Vector3 waypointDTPos = _waypointDT.GetComponent<Transform> ().position;
		WaypointDT neighbourDT = _neighbourDT.GetComponent<WaypointDT> ();
		Vector3 neighbourPos = _neighbourDT.GetComponent<Transform> ().position;

		Direction suitableDirection = MapController.GetPointDirection(waypointDTPos, neighbourPos);
		suitableDirection = MapController.GetOppositeDirection (suitableDirection);

		if (suitableDirection == neighbourDT.ladderDirection) {
			return true;
		}

		return false;
	}

	static void SetIcon (GameObject _waypointDTInstance) {
		IconManager.SetIcon (_waypointDTInstance, IconManager.Icon.DiamondGray);
		WaypointDT _waypointDT = _waypointDTInstance.GetComponent<WaypointDT>();

		if (!_waypointDT.GetComponent<SphereCollider>().enabled)
			IconManager.SetIcon (_waypointDTInstance, IconManager.Icon.DiamondRed);
		
		if (_waypointDT.triggers != null && _waypointDT.triggers.Count > 0)
			IconManager.SetIcon (_waypointDTInstance, IconManager.Icon.DiamondYellow);
	}

	static void RemoveWrongTriggers (WaypointDT _waypointDT) {
		if (_waypointDT.triggers != null) {
			for (int i = 0; i < _waypointDT.triggers.Count; i++) {
				if (_waypointDT.triggers [i] == null) {
					continue;
				}
				if (_waypointDT.triggers [i].GetComponent<TriggerDT> () == null) {
					_waypointDT.triggers [i] = null;
				}
			}
		}
	}

	public static void UpdateTriggers () {
		GameObject[] items = MapController.SetContainer (MapController.TAG_TRIGGER);

		foreach (var item in items) {
			TriggerDT triggerDT = item.GetComponent<TriggerDT> ();

			if (triggerDT.activateWaypoints.Length == 0) {
				Debug.LogError ("No activate waypoints in trigger: " + triggerDT.GetComponent<Transform> ().position);
			}
			if (triggerDT.path.Length == 0) {
				triggerDT.path = new GameObject[1];
			}
			Vector3 triggerDTPos = triggerDT.GetComponent<Transform> ().position;
			foreach (var _waypoint in waypoints) {
				Vector3 waypointDTPos = _waypoint.GetComponent<Transform> ().position;
				if (triggerDTPos == waypointDTPos) {
					triggerDT.path [0] = _waypoint.gameObject;
				}
			}

			EditorUtility.SetDirty (triggerDT);
		}
	}

	static void DrawWaypointNetwork () {
		if (waypoints == null) {
			return;
		}

		foreach (var _waypoint in waypoints) {
			if (_waypoint == null) {
				continue;
			}
			WaypointDT waypointObject = _waypoint.GetComponent<WaypointDT> ();
			Transform waypointTransform = _waypoint.GetComponent<Transform> ();

			foreach (var _neigbour in _waypoint.GetComponent<WaypointDT> ().neighbours) {

				WaypointDT neigbourObject = _neigbour.GetComponent<WaypointDT> ();
				Transform neigbourTransform = _neigbour.GetComponent<Transform> ();

				if (!waypointObject.GetComponent<SphereCollider>().enabled || !neigbourObject.GetComponent<SphereCollider>().enabled ) {
					Handles.color = Color.red;
				} else{
					Handles.color = Color.white;
				}

				Vector3 middlePoint = Vector3.Lerp(waypointTransform.position, neigbourTransform.position, 0.45f);
				Handles.DrawLine (waypointTransform.position, middlePoint);
			}
		}
	}

	static void RemoveEmpties (List<GameObject> _source) {
		if (_source == null) {
			return;
		}

		for(var i = _source.Count - 1; i > -1; i--) {
			if (_source[i] == null) {
				_source.RemoveAt (i);
			}
		}
	}
}