using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(WaypointDT))]
public class WaypointDTEditor : Editor {
	
	WaypointDT waypointDT;
	Transform waypointDTTransform;

	public override void OnInspectorGUI() {
		waypointDT = (WaypointDT) target;
		waypointDTTransform = waypointDT.GetComponent<Transform>();

		// Draw UI
		UnitModelUIControl ();
		DefaultUIControl ("neighbours");
		DefaultUIControl ("triggers");
		DefaultUIControl ("noRepeat");
		DefaultUIControl ("activateOnTouch");
		DefaultUIControl ("modelColliderEnabled");
		WaypointTypeUIControl ();

		// Save settings
		EditorSceneManager.MarkAllScenesDirty ();
		EditorUtility.SetDirty (waypointDT);
	}

	void UnitModelUIControl () {
		GameObject model = (GameObject)EditorGUILayout.ObjectField ("Unit Model:", waypointDT.unitModel, typeof(GameObject), false);
		if (model != waypointDT.unitModel) {
			waypointDT.unitModel = model;

			if (waypointDT.unitModel == null) {
				RemoveChild ();
				return;
			}

			waypointDT.unitPrefab = AssetDatabase.GetAssetPath(model);
			waypointDT.unitPrefab = waypointDT.unitPrefab.Substring(0, waypointDT.unitPrefab.Length - 7);
			waypointDT.unitPrefab = waypointDT.unitPrefab.Substring(17);
			SetModel ();
		}

		if (waypointDT.unitModel != null) {
			UnitRotationUIControl ();
		}
	}

	void RemoveChild () {
		if (waypointDTTransform.childCount > 0) {
			GameObject.DestroyImmediate (waypointDTTransform.GetChild (0).gameObject);
		}
	}

	void SetModel () {
		RemoveChild ();

		// Instant new children
		GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab (waypointDT.unitModel);
		Transform newChildTransform = newChild.GetComponent<Transform> ();
		newChildTransform.SetParent (waypointDTTransform);
		newChildTransform.localPosition = Vector3.zero;
		newChildTransform.eulerAngles = MapController.GetEulerAngle(waypointDT.unitDirection);
		newChildTransform.tag = "Untagged";
	}

	void DefaultUIControl (string _property) {
		serializedObject.Update();
		EditorGUIUtility.LookLikeInspector();
		SerializedProperty tps = serializedObject.FindProperty (_property);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(tps, true);

		if(EditorGUI.EndChangeCheck()) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUIUtility.LookLikeControls();
	}

	void UnitRotationUIControl () {
		Direction rotation = (Direction)EditorGUILayout.EnumPopup ("Unit Rotation:", waypointDT.unitDirection);

		if (rotation != waypointDT.unitDirection) {
			waypointDT.unitDirection = rotation;
			SetModel ();
		}
	}

	void WaypointTypeUIControl () {
		WaypointsTypes waypointType = (WaypointsTypes)EditorGUILayout.EnumPopup ("Waypoint Type:", waypointDT.Type);

		if (waypointType != waypointDT.Type) {
			waypointDT.Type = waypointType;
		}

		if (waypointDT.Type == WaypointsTypes.Ladder) {
			Direction ladderDirection = (Direction)EditorGUILayout.EnumPopup ("Ladder Bottom:", waypointDT.ladderDirection);

			if (ladderDirection != waypointDT.ladderDirection) {
				waypointDT.ladderDirection = ladderDirection;
			}
		}
	}
}
