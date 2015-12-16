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


		waypointField = (GameObject)EditorGUILayout.ObjectField("WaypointDT", waypointField,  typeof(GameObject), false);
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

		DrawWaypointNetwork ();

		if (GUILayout.Button ("Update level")) {
			UpdateTiles ();
			UpdateWaypoints ();
			UpdateTriggers ();
		}
	}

	static void UpdateTiles () {
		MapController.GetContainer (MapController.TAG_TILE);
	}

	void CreateWaypoints () {
		GameObject[] items = Selection.gameObjects;

		foreach (var _item in items) {
			GameObject waypoint = GameObject.Instantiate (waypointField);
			waypoint.GetComponent<Transform> ().position = _item.GetComponent<Transform> ().position;
		}
	}

	static void UpdateWaypoints () {
		GameObject[] items = MapController.GetContainer (MapController.TAG_WAYPOINT);
		waypoints = items;

		for (int a = 0; a < items.Length; a++) {
			WaypointDT pointA = items [a].GetComponent<WaypointDT> ();

			Transform pointATransform = pointA.GetComponent<Transform> ();
			Vector3 pointAposition = pointATransform.position;
			pointA.neighbours = new List<GameObject>();

			for (int b = 0; b < items.Length; b++) {
				WaypointDT pointB = items [b].GetComponent<WaypointDT> ();

				if (pointA == pointB) {
					continue;
				}

				Vector3 pointBposition = pointB.GetComponent<Transform> ().position;
				float distance = Vector3.Distance (pointAposition, pointBposition);

				if (distance < neighbourDistance) {
					pointA.neighbours.Add(items [b]);
				}
			}

			// Check triggers
			if (pointA.triggers != null) {
				for (int i = 0; i < pointA.triggers.Count; i++) {
					if (pointA.triggers[i] == null){
						continue;
					}
					if (pointA.triggers[i].GetComponent<TriggerDT> () == null) {
						pointA.triggers[i] = null;
					}
				}
			}
			RemoveEmpties (pointA.triggers);

			// Set icon
			IconManager.SetIcon (items [a], IconManager.Icon.DiamondGray);

			if (!pointA.walkable)
				IconManager.SetIcon (items [a], IconManager.Icon.DiamondRed);

			if (pointA.triggers != null && pointA.triggers.Count > 0)
				IconManager.SetIcon (items [a], IconManager.Icon.DiamondYellow);

			// Save changes
			EditorUtility.SetDirty(pointA);
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