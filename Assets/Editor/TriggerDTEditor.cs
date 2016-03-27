using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TriggerObjectDT))]
public class TriggerDTEditor : Editor {
	
	TriggerObjectDT triggerObjectDT;
	Transform triggerDTTransform;
	TriggerObjectDT triggerListDT;
	List<int> activeTriggersID;

	public override void OnInspectorGUI() {
		triggerObjectDT = (TriggerObjectDT) target;

		triggerDTTransform = triggerObjectDT.GetComponent<Transform>();

		// Draw UI
		DefaultUIControl ("isList");

		if (triggerObjectDT.isList)
		{
			DefaultUIControl ("triggersList");
		}
		else 
		{
			UpdateTriggersList ();
			if(GUILayout.Button("Create dynamic object"))
			{
				Create();
			}
			DefaultUIControl ("triggerActiveList");
			DefaultUIControl ("localTriggersItems");

			UpdateTriggersItems ();
		}

//		ModelUIControl ();
//		DefaultUIControl ("prefab");
//		DefaultUIControl ("activateNodes");

//		if (triggerDT.path == null || triggerDT.path.Length == 0) {
//			triggerDT.path = new GameObject[]{triggerDT.gameObject};
//		} else {
//			triggerDT.path [0] = triggerDT.gameObject;
//		}

//		DefaultUIControl ("path");
//		DefaultUIControl ("removeOnActivation");

//		if(GUILayout.Button("Destroy dynamic object"))
//		{
//			Destroy();
//		}

		// Save settings
		EditorSceneManager.MarkAllScenesDirty ();
		EditorUtility.SetDirty (triggerObjectDT);
	}

	void UpdateTriggersList() {
		// Get TagList object
		GameObject[] triggerListObject = MapController.SetContainer (MapController.TAG_TRIGGER);
		triggerListDT = triggerListObject [0].GetComponent<TriggerObjectDT> ();

		// Create new bool array of Active Triggers
		bool[] newTriggerActiveList;
		if (triggerListDT.triggersList == null) {
			newTriggerActiveList = new bool[0];
		}
		newTriggerActiveList = new bool[triggerListDT.triggersList.Count];
		activeTriggersID = new List<int> ();
		triggerObjectDT.localTriggersItems = new List<TriggerDT> ();

		// Copy old bools to new array
		for (int i = 0; i < triggerObjectDT.triggerActiveList.Length; i++) {
			if (newTriggerActiveList.Length < triggerObjectDT.triggerActiveList.Length) {
				break;
			}
			newTriggerActiveList [i] = triggerObjectDT.triggerActiveList [i];
			if (newTriggerActiveList [i]) {
				activeTriggersID.Add (i);
				triggerObjectDT.localTriggersItems.Add (triggerListDT.triggersList [i]);
			}
		}
		triggerObjectDT.triggerActiveList = newTriggerActiveList;
	}

	void UpdateTriggersItems ()
	{
		for (int i = 0; i < triggerObjectDT.localTriggersItems.Count; i++) {
			triggerListDT.triggersList [activeTriggersID [i]] = triggerObjectDT.localTriggersItems [i];
		}
	}

//	void ModelUIControl () {
//		GameObject model = (GameObject)EditorGUILayout.ObjectField ("Model:", triggerDT.model, typeof(GameObject), false);
//
//		if (model != triggerObjectDT.model) {
//			triggerObjectDT.model = model;
//
//			triggerObjectDT.prefab = AssetDatabase.GetAssetPath(model);
//			triggerObjectDT.prefab = triggerObjectDT.prefab.Substring(0, triggerObjectDT.prefab.Length - 7);
//			triggerObjectDT.prefab = triggerObjectDT.prefab.Substring(17);
//
//			SetModel ();
//		}
//
//		if (triggerObjectDT.model != null) {
//			TriggerRotationUIControl ();
//		}
//	}
//
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
//
//	void RemoveChild ()
//	{
//		if (triggerDTTransform.childCount > 0) {
//			GameObject.DestroyImmediate (triggerDTTransform.GetChild (0).gameObject);
//		}
//	}
//
//	void SetModel () {
//		// Remove old children
//		RemoveChild ();
//
//		// Instant new children
//		GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab (triggerObjectDT.model);
//		Transform newChildTransform = newChild.GetComponent<Transform> ();
//		newChildTransform.SetParent (triggerDTTransform);
//		newChildTransform.localPosition = Vector3.zero;
//		newChildTransform.eulerAngles = MapController.GetEulerAngle(triggerObjectDT.modelDirection);
//		newChildTransform.tag = "Untagged";
//	}
//
//	void TriggerRotationUIControl () {
//		Direction rotation = (Direction)EditorGUILayout.EnumPopup ("Model Rotation:", triggerObjectDT.modelDirection);
//
//		if (rotation != triggerObjectDT.modelDirection) {
//			triggerObjectDT.modelDirection = rotation;
//			SetModel ();
//		}
//	}

	void Destroy()
	{
//		RemoveChild ();
		DestroyImmediate (triggerObjectDT);
	}

	void Create()
	{
		if (triggerListDT.triggersList == null) {
			triggerListDT.triggersList = new List<TriggerDT> ();
		}
		triggerListDT.triggersList.Add(new TriggerDT());
		UpdateTriggersList ();
		triggerObjectDT.triggerActiveList[triggerObjectDT.triggerActiveList.Length - 1] = true;
	}
}
