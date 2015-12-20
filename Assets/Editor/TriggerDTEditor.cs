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
		DefaultUIControl ("waypoints");
		DefaultUIControl ("trigger");
		SetCoordinates ();

		// Save settings
		EditorUtility.SetDirty (triggerDT);
	}

	void ModelUIControl () {
		GameObject model = (GameObject)EditorGUILayout.ObjectField ("Model:", triggerDT.trigger.model, typeof(GameObject), false);
		if (model != triggerDT.trigger.model) {
			triggerDT.trigger.model = model;
			triggerDT.trigger.prefab = AssetDatabase.GetAssetPath(triggerDT.trigger.model);
			triggerDT.trigger.prefab = triggerDT.trigger.prefab.Substring(0, triggerDT.trigger.prefab.Length - 7);
			triggerDT.trigger.prefab = triggerDT.trigger.prefab.Substring(17);
			SetModel ();
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

	void SetCoordinates () {
		if (triggerDT.trigger.coordinates == null) {
			triggerDT.trigger.coordinates = new Vector3[1];
		}
		triggerDT.trigger.coordinates [0] = triggerDTTransform.position;
	}

	void SetModel () {
		// Remove old children
		if (triggerDTTransform.childCount > 0) {
			GameObject.DestroyImmediate (triggerDTTransform.GetChild (0).gameObject);
		}

		// Instant new children
		GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab (triggerDT.trigger.model);
		Transform newChildTransform = newChild.GetComponent<Transform> ();
		newChildTransform.SetParent (triggerDTTransform);
		newChildTransform.localPosition = Vector3.zero;
		newChildTransform.tag = "Untagged";
	}
}
