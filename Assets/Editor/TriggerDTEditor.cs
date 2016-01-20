using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TriggerDT))]
public class TriggerDTEditor : Editor {
	
	TriggerDT triggerDT;
	Transform triggerDTTransform;

	public override void OnInspectorGUI() {
		triggerDT = (TriggerDT) target;

		triggerDTTransform = triggerDT.GetComponent<Transform>();

		// Draw UI
		ModelUIControl ();
		DefaultUIControl ("prefab");
		DefaultUIControl ("activateWaypoints");
		DefaultUIControl ("path");
		DefaultUIControl ("removeOnActivation");

		// Save settings
		EditorSceneManager.MarkAllScenesDirty ();
		EditorUtility.SetDirty (triggerDT);
	}

	void ModelUIControl () {
		GameObject model = (GameObject)EditorGUILayout.ObjectField ("Model:", triggerDT.model, typeof(GameObject), false);

		if (model != triggerDT.model) {
			triggerDT.model = model;

			triggerDT.prefab = AssetDatabase.GetAssetPath(model);
			triggerDT.prefab = triggerDT.prefab.Substring(0, triggerDT.prefab.Length - 7);
			triggerDT.prefab = triggerDT.prefab.Substring(17);

			SetModel ();
		}

		if (triggerDT.model != null) {
			TriggerRotationUIControl ();
		}
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

	void SetModel () {
		// Remove old children
		if (triggerDTTransform.childCount > 0) {
			GameObject.DestroyImmediate (triggerDTTransform.GetChild (0).gameObject);
		}

		// Instant new children
		GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab (triggerDT.model);
		Transform newChildTransform = newChild.GetComponent<Transform> ();
		newChildTransform.SetParent (triggerDTTransform);
		newChildTransform.localPosition = Vector3.zero;
		newChildTransform.eulerAngles = MapController.GetEulerAngle(triggerDT.tileDirection);
		newChildTransform.tag = "Untagged";
	}

	void TriggerRotationUIControl () {
		Direction rotation = (Direction)EditorGUILayout.EnumPopup ("Trigger Rotation:", triggerDT.tileDirection);

		if (rotation != triggerDT.tileDirection) {
			triggerDT.tileDirection = rotation;
			SetModel ();
		}
	}
}
