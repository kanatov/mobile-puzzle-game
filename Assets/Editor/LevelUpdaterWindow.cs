using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
class LevelUpdaterWindow : EditorWindow {
	static LevelUpdaterWindow window;

	// Waypoints
	public GameObject instanceObject;

	// Window
	[MenuItem ("Tools/Level Updater")]
	public static void OpenWindow(){
		window = (LevelUpdaterWindow)EditorWindow.GetWindow(typeof(LevelUpdaterWindow));
		GUIContent titleContent = new GUIContent ("Level Updater");
		window.titleContent = titleContent;
	}

	void OnGUI(){
		if(window == null) {
			OpenWindow();
		}

		// Draw filed for waypoint place
		instanceObject = (GameObject)EditorGUILayout.ObjectField(
			"WaypointDT",
			instanceObject, 
			typeof(GameObject),
			false
		);

		// Create waypoints button
		if (GUILayout.Button ("Create waypoints")) {
			if (Selection.gameObjects.Length == 0) {
				return;
			}
			if (instanceObject == null) {
				return;
			}

			LevelUpdater.CreateWaypoints (instanceObject);
			LevelUpdater.UpdateWaypoints ();
		}

		// Update level button
		if (GUILayout.Button ("Update level")) {
			LevelUpdater.UpdateTiles ();
			LevelUpdater.UpdateWaypoints ();
			LevelUpdater.UpdateTriggers ();
		}
	}
}