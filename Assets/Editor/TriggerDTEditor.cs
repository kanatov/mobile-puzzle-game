using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TriggerDT))]
public class TriggerDTEditor : Editor {
	TriggerDT triggerDT;
	Transform triggerDTTransform;
	bool collapsed;
	int listSize;

	public override void OnInspectorGUI() {
		triggerDT = (TriggerDT) target;
		triggerDT.state = (int)EditorGUILayout.IntField("State:",triggerDT.state);
		TriggerType ();
		WaypointsArray ();

	}

	void TriggerType () {
		TriggersTypes type = (TriggersTypes)EditorGUILayout.EnumPopup ("Type:", triggerDT.type);
		if (type != triggerDT.type) {
			triggerDT.type = type;
			ApplyChanges ();
		}
	}

	void WaypointsArray () {
		serializedObject.Update();
		EditorGUIUtility.LookLikeInspector();
		SerializedProperty tps = serializedObject.FindProperty ("waypoints");
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(tps, true);
		if(EditorGUI.EndChangeCheck()) {
			serializedObject.ApplyModifiedProperties ();
		}
		
		EditorGUIUtility.LookLikeControls();
	}
	
	void ApplyChanges() {
		triggerDTTransform = triggerDT.GetComponent<Transform>();
		
		if (triggerDTTransform.childCount > 0) {
			GameObject.DestroyImmediate(triggerDTTransform.GetChild(0).gameObject);
		}

		GameObject newChild = GameObject.Instantiate(MapController.triggersModels[(int)triggerDT.type]);
		Transform newChildTransform = newChild.GetComponent<Transform>();
		newChildTransform.SetParent(triggerDTTransform);
		newChildTransform.localPosition = Vector3.zero;
		newChildTransform.tag = "Untagged";

		EditorUtility.SetDirty(triggerDT);
	} 
}
