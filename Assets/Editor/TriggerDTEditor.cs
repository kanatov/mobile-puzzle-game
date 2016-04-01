using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(TriggerDTViewer))]
public class TriggerDTEditor : Editor
{
	
	TriggerDTViewer triggerDTCur;
	Transform triggerDTCurTrans;
	string newTriggerName = "";
	GameObject[] nodes;

	void OnEnable ()
	{
		triggerDTCur = (TriggerDTViewer)target;
		triggerDTCurTrans = triggerDTCur.GetComponent<Transform> ();
	}

	public override void OnInspectorGUI ()
	{
		FetchTriggersList ();
		DrawList ();
		Create ();
		UpdateTriggersList ();

		// Save settings
		EditorSceneManager.MarkAllScenesDirty ();
		EditorUtility.SetDirty (triggerDTCur);

		// Destroy class
		GUILayout.Space (10f);
		if(GUILayout.Button("Destroy dynamic object")) {Destroy();}
	}

	// Init TriggersList from network
	void FetchTriggersList ()
	{
		if (nodes == null)
		{
			nodes = MapController.SetContainer (MapController.TAG_NODE);
		}

		if (triggerDTCur.triggersList == null) {

			// Create empty list
			triggerDTCur.triggersList = new List<TriggerDT> ();

			// Try to copy from network
			for (int i = 0; i < nodes.Length; i++) {
				if (triggerDTCur.gameObject == nodes [i]) {
					continue;
				}
				TriggerDTViewer triggerDTViewer = nodes [i].GetComponent<TriggerDTViewer> ();
				if (triggerDTViewer != null) {
					triggerDTCur.triggersList = triggerDTViewer.triggersList;
					break;
				}
			}
		}
	}

	void DrawList ()
	{
		for (int i = 0; i < triggerDTCur.triggersList.Count; i++)
		{
			GUILayout.BeginVertical (EditorStyles.helpBox);

			// Is active check
			bool isActive = CheckActive (triggerDTCur.triggersList[i].name);
			bool isActiveCheck = isActive;

			GUILayout.BeginHorizontal ();
			isActiveCheck = EditorGUILayout.Toggle (
				triggerDTCur.triggersList [i].name,
				isActiveCheck
			);

			if(GUILayout.Button("×")) {RemoveTrigger(i);}
			GUILayout.EndHorizontal ();
			if (isActiveCheck != isActive)
			{
				if (isActiveCheck)
				{
					triggerDTCur.activeTriggers.Add (triggerDTCur.triggersList [i].name);
				}
				else
				{
					triggerDTCur.activeTriggers.Remove (triggerDTCur.triggersList [i].name);
				}
			}

			// Draw trigger body if active
			if (isActiveCheck)
			{
				triggerDTCur.triggersList [i].name = EditorGUILayout.TextField ("Name: ", triggerDTCur.triggersList [i].name);
				ModelPicker (i);

				triggerDTCur.triggersList [i].removeOnActivation = EditorGUILayout.Toggle (
					"Remove on actiovation: ",
					triggerDTCur.triggersList [i].removeOnActivation
				);
			}

			// Populate path
			if (triggerDTCur.triggersList [i].path == null || triggerDTCur.triggersList [i].path.Length == 0)
			{
				triggerDTCur.triggersList [i].path = new GameObject[]{triggerDTCur.gameObject};
			}
			else
			{
				triggerDTCur.triggersList [i].path [0] = triggerDTCur.gameObject;
			}

			EditorGUILayout.EndVertical ();
		}
	}

	bool CheckActive (string name)
	{
		for (int i = 0; i < triggerDTCur.activeTriggers.Count; i++) {
			if (triggerDTCur.activeTriggers [i] == name) {
				return true;
			}
		}
		return false;
	}

	void ModelPicker (int i)
	{
		GameObject model = (GameObject)EditorGUILayout.ObjectField ("Model:", triggerDTCur.triggersList [i].model, typeof(GameObject), false);

		// Prefab path
		if (model != triggerDTCur.triggersList [i].model) {
			triggerDTCur.triggersList [i].model = model;

			triggerDTCur.triggersList [i].prefab = AssetDatabase.GetAssetPath (model);
			triggerDTCur.triggersList [i].prefab = triggerDTCur.triggersList [i].prefab.Substring (0, triggerDTCur.triggersList [i].prefab.Length - 7);
			triggerDTCur.triggersList [i].prefab = triggerDTCur.triggersList [i].prefab.Substring (17);

			SetModel (i);
		}

		if (triggerDTCur.triggersList [i].model != null) {
			TriggerRotationUIControl (i);
		}

	}

	void RemoveTrigger (int n) {
		triggerDTCur.triggersList.RemoveAt (n);
	}

	void RemoveChild ()
	{
		while (triggerDTCurTrans.childCount > 0) {
			GameObject.DestroyImmediate (triggerDTCurTrans.GetChild (0).gameObject);
		}
	}

	void SetModel (int i)
	{
		// Remove old children
		RemoveChild ();
	
		// Instant new children
		GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab (triggerDTCur.triggersList [i].model);
		Transform newChildTransform = newChild.GetComponent<Transform> ();
		newChildTransform.SetParent (triggerDTCurTrans);
		newChildTransform.localPosition = Vector3.zero;
		newChildTransform.eulerAngles = MapController.GetEulerAngle (triggerDTCur.triggersList [i].modelDirection);
		newChildTransform.tag = "Untagged";
	}

	void TriggerRotationUIControl (int i)
	{
		Direction rotation = (Direction)EditorGUILayout.EnumPopup ("Model Rotation:", triggerDTCur.triggersList [i].modelDirection);
	
		if (rotation != triggerDTCur.triggersList [i].modelDirection) {
			triggerDTCur.triggersList [i].modelDirection = rotation;
			SetModel (i);
		}
	}

	void Destroy ()
	{
		RemoveChild ();
		DestroyImmediate (triggerDTCur);
	}

	void Create ()
	{
		GUILayout.BeginVertical (EditorStyles.helpBox);
		if (newTriggerName == "") {
			newTriggerName = "Trigger #" + triggerDTCur.triggersList.Count;
		}
		newTriggerName = EditorGUILayout.TextField ("Name: ", newTriggerName);
		if (GUILayout.Button ("Create")) {
			CreateButton ();
		}
		GUILayout.EndVertical();
	}

	void CreateButton()
	{
		triggerDTCur.triggersList.Add (new TriggerDT ());
		triggerDTCur.triggersList[triggerDTCur.triggersList.Count - 1].name = newTriggerName;

		newTriggerName = "";
	}

	void UpdateTriggersList ()
	{
		// Send current TriggersList to everyone
		for (int i = 0; i < nodes.Length; i++) {
			TriggerDTViewer triggerDTViewer = nodes [i].GetComponent<TriggerDTViewer> ();
			if (triggerDTViewer) {
				triggerDTViewer.triggersList = triggerDTCur.triggersList;
			}
		}
	}
}
