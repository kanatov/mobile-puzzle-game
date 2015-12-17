using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
class LevelUpdater : EditorWindow {
	static LevelUpdater window;

	// Waypoints
	static GameObject[] waypoints;
	public GameObject waypointField;
	static float neighbourDistance = 1.2f;

	// Window
	[MenuItem ("Tools/Level Updater")]
	public static void OpenWindow(){
		window = (LevelUpdater)EditorWindow.GetWindow(typeof(LevelUpdater));
		GUIContent titleContent = new GUIContent ("Level Updater");
		window.titleContent = titleContent;
	}
	
	void OnGUI(){
		if(window == null) {
			OpenWindow();
		}

		// Draw filed for waypoint place
		waypointField = (GameObject)EditorGUILayout.ObjectField(
			"WaypointDT",
			waypointField, 
			typeof(GameObject),
			false
		);

		// Create waypoints button
		if (GUILayout.Button ("Create waypoints")) {
			if (Selection.gameObjects.Length == 0) {
				return;
			}
			if (waypointField == null) {
				return;
			}

			CreateWaypoints ();
			UpdateWaypoints ();
		}

		// Update level button
		if (GUILayout.Button ("Update level")) {
			UpdateTiles ();
			UpdateWaypoints ();
			UpdateTriggers ();
		}
	}

	void CreateWaypoints () {
		GameObject[] items = Selection.gameObjects;

		foreach (var _item in items) {
			GameObject waypoint = GameObject.Instantiate (waypointField);
			waypoint.GetComponent<Transform> ().position = _item.GetComponent<Transform> ().position;
		}
	}

	// Place tiles to container
	static void UpdateTiles () {
		MapController.GetContainer (MapController.TAG_TILE);
	}

	// Place waypoint to container
	// Update waypoint data
	static void UpdateWaypoints () {
		waypoints = MapController.GetContainer (MapController.TAG_WAYPOINT);

		for (int a = 0; a < waypoints.Length; a++) {
			WaypointDT _waypointDT = waypoints[a].GetComponent<WaypointDT> ();
			SetNeighbours (_waypointDT);

			// Check triggers
			RemoveNullTriggers (_waypointDT);
			RemoveEmpties (_waypointDT.triggers);

			// Set icon
			SetIcon (waypoints [a]);

			// Save changes
			EditorUtility.SetDirty(_waypointDT);
		}

		DrawWaypointNetwork ();
	}

	static void SetNeighbours (WaypointDT _waypointDT) {
		Transform pointATransform = _waypointDT.GetComponent<Transform> ();
		Vector3 pointAPosition = pointATransform.position;

		_waypointDT.neighbours = new List<GameObject> ();
		_waypointDT.rotations = new List<UnitRotation> ();

		for (int b = 0; b < waypoints.Length; b++) {
			WaypointDT pointB = waypoints [b].GetComponent<WaypointDT> ();

			if (_waypointDT == pointB) {
				continue;
			}

			Transform pointBTransform = pointB.GetComponent<Transform> ();
			Vector3 pointBPosition = pointBTransform.position;
			float distance = Vector3.Distance (pointAPosition, pointBPosition);

			if (distance < neighbourDistance) {
				_waypointDT.neighbours.Add (waypoints [b]);
//				_waypointDT.rotations.Add (GetRotation (pointATransform, pointBTransform));
			}
		}
	}

//	static UnitRotation GetRotation (Transform _source, Transform _target) {
//		Vector3 targetRotation = Quaternion.LookRotation(_target.position - _source.position, Vector3.up);
//		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0);    
//
//
//		angle = Mathf.Round(angle / 60);
//		Debug.Log (angle);
//		return (UnitRotation) angle;
//	}

	static void SetIcon (GameObject _waypointDTInstance) {
		IconManager.SetIcon (_waypointDTInstance, IconManager.Icon.DiamondGray);
		WaypointDT _waypointDT = _waypointDTInstance.GetComponent<WaypointDT>();

		if (!_waypointDT.walkable)
			IconManager.SetIcon (_waypointDTInstance, IconManager.Icon.DiamondRed);
		
		if (_waypointDT.triggers != null && _waypointDT.triggers.Count > 0)
			IconManager.SetIcon (_waypointDTInstance, IconManager.Icon.DiamondYellow);
	}

	static void RemoveNullTriggers (WaypointDT _waypointDT) {
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

	static void UpdateTriggers () {
		GameObject[] items = MapController.GetContainer (MapController.TAG_TRIGGER);

		foreach (var item in items) {
			TriggerDT triggerDT = item.GetComponent<TriggerDT> ();
			RemoveEmpties (triggerDT.waypoints);
			EditorUtility.SetDirty (triggerDT);
		}
	}

	static void DrawWaypointNetwork () {
		if (waypoints == null) {
			return;
		}

		List<GameObject> completed = new List<GameObject>();

		foreach (var _waypoint in waypoints) {
			if (_waypoint == null) {
				continue;
			}

			WaypointDT waypointObject = _waypoint.GetComponent<WaypointDT> ();
			Transform waypointTransform = _waypoint.GetComponent<Transform> ();

			foreach (var _neigbour in _waypoint.GetComponent<WaypointDT> ().neighbours) {
				if (completed.Contains(_neigbour)) {
					continue;
				}

				WaypointDT neigbourObject = _neigbour.GetComponent<WaypointDT> ();
				Transform neigbourTransform = _neigbour.GetComponent<Transform> ();
				Color lineColor;

				if (!waypointObject.walkable || !neigbourObject.walkable ) {
					lineColor = Color.red;
				} else{
					lineColor = Color.white;
				}

				Debug.DrawLine (waypointTransform.position, neigbourTransform.position, lineColor, 1f);
			}

			completed.Add(_waypoint);
		}
	}

	static void RemoveEmpties (List<GameObject> _source) {
		if (_source == null) {
			return;
		}

		for(var i = _source.Count - 1; i > -1; i--) {
			if (_source[i] == null)
				_source.RemoveAt(i);
		}
	}
}