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
		UnitRotationUIControl ();
		DefaultUIControl ("neighbours");
		DefaultUIControl ("triggers");
		DefaultUIControl ("walkable");

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
		newChildTransform.eulerAngles = MapController.GetEulerAngle(waypointDT.unitRotation);
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
		UnitRotation rotation = (UnitRotation)EditorGUILayout.EnumPopup ("Unit Rotation:", waypointDT.unitRotation);

		if (rotation != waypointDT.unitRotation) {
			waypointDT.unitRotation = rotation;
			SetModel ();
		}
	}
}
