﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(NodeDT))]
public class NodeDTEditor : Editor {
	
	NodeDT nodeDTCur;
	Transform nodeDTTrans;

	string newTriggerName = "";

	GameObject[] nodes;

	void OnEnable ()
	{
		nodeDTCur = (NodeDT) target;
		nodeDTTrans = nodeDTCur.GetComponent<Transform>();
	}

	public override void OnInspectorGUI()
	{
		// Draw UI
		DefaultUIControl ("walkNodes");
		DefaultUIControl ("localNodes");
		DefaultUIControl ("singleActivation");
		DefaultUIControl ("touchActiovation");
		DefaultUIControl ("walk");
		NodeTypeUIControl ();

		// Triggers UI
		GUILayout.Space (15f);
		EditorGUILayout.LabelField ("Triggers", EditorStyles.boldLabel);
		DefaultUIControl ("triggers");
		FetchTriggersList ();
		DrawList ();
		GUILayout.Space (15f);
		Create ();
		UpdateTriggersList ();

		// Save settings
		EditorSceneManager.MarkAllScenesDirty ();
		EditorUtility.SetDirty (nodeDTCur);

		// Display all active triggers
		//		DisplayActiveTriggers ();

		// Remove triggers
		GUILayout.Space (15f);
		if(GUILayout.Button("Remove triggers from this NodeDT", GUILayout.Height(23f))) {Destroy();}
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

	void NodeTypeUIControl () {
		NodeTypes nodeType = (NodeTypes)EditorGUILayout.EnumPopup ("Neighbour Type:", nodeDTCur.type);

		if (nodeType != nodeDTCur.type) {
			nodeDTCur.type = nodeType;
		}

		if (nodeDTCur.type == NodeTypes.Ladder) {
			Direction ladderDirection = (Direction)EditorGUILayout.EnumPopup ("Ladder Base:", nodeDTCur.ladderDirection);

			if (ladderDirection != nodeDTCur.ladderDirection) {
				nodeDTCur.ladderDirection = ladderDirection;
			}
		}
	}

	// Init TriggersList from network
	void FetchTriggersList ()
	{
		if (nodes == null)
		{
			nodes = MapController.SetContainer (MapController.TAG_NODE);
		}

		if (nodeDTCur.triggersList == null)
		{

			// Create empty list
			nodeDTCur.triggersList = new List<TriggerDT> ();

			// Try to copy from network
			for (int i = 0; i < nodes.Length; i++)
			{
				if (nodeDTCur.gameObject == nodes [i])
				{
					continue;
				}
				nodeDTCur.triggersList = nodes[i].GetComponent<NodeDT>().triggersList;
				break;
			}
		}
	}

	void DrawList ()
	{
		for (int i = 0; i < nodeDTCur.triggersList.Count; i++)
		{
			GUILayout.BeginVertical (EditorStyles.helpBox);

			// Is active check
			bool isActive = CheckActive (nodeDTCur.triggersList[i].name);
			bool isActiveCheck = isActive;

			GUILayout.BeginHorizontal (EditorStyles.toolbar);

			isActiveCheck = EditorGUILayout.Toggle (isActiveCheck, GUILayout.MaxWidth(20f));
			EditorGUILayout.LabelField (nodeDTCur.triggersList [i].name, EditorStyles.boldLabel);
			GUILayout.FlexibleSpace ();

			if(GUILayout.Button("×",  GUILayout.MaxWidth(40f))) {RemoveTrigger(i);}
			GUILayout.EndHorizontal ();

			if (isActiveCheck != isActive)
			{
				if (isActiveCheck)
				{
					AddTriggerToNode (nodeDTCur.triggersList [i].name);
				}
				else
				{
					RemoveTriggerFromNode (nodeDTCur.triggersList [i].name);
				}
			}

			// Draw trigger body if active
			if (isActiveCheck)
			{
				// Type
				nodeDTCur.triggersList [i].type = (TriggerTypes)EditorGUILayout.EnumPopup (
					"Type: ",
					nodeDTCur.triggersList [i].type
				);

				// Model
				ModelPicker (i);

				// Populate path
				if (nodeDTCur.triggersList [i].path == null || nodeDTCur.triggersList [i].path.Count == 0)
				{
					nodeDTCur.triggersList [i].path = new List<GameObject>(){nodeDTCur.gameObject};
				}

				CustomList (nodeDTCur.triggersList [i].path, "Waypoints path");
				CustomList (nodeDTCur.triggersList [i].activateNodes, "Activate waypoints");



			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space ();
		}
	}

	void CustomList (List<GameObject> _list, string _label)
	{
		// Draw waypoints list
		EditorGUILayout.Space ();
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		EditorGUILayout.BeginHorizontal ();
		// Label
		EditorGUILayout.LabelField (_label + ": " + _list.Count, EditorStyles.boldLabel);

		if (GUILayout.Button ("Add"))
		{
			_list.Add (null);
		}
		EditorGUILayout.EndHorizontal ();

		// Array
		for (int j = 0; j < _list.Count; j++)
		{
			EditorGUILayout.BeginHorizontal ();
			_list [j] = (GameObject)EditorGUILayout.ObjectField (_list [j], typeof(UnityEngine.Object), true);

			if (GUILayout.Button ("×", GUILayout.MaxWidth (40f)))
			{
				_list.RemoveAt (j);
			}
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.EndVertical ();
	}

	bool CheckActive (string name)
	{
		for (int i = 0; i < nodeDTCur.triggers.Count; i++)
		{
			if (nodeDTCur.triggers [i] == name) {return true;}
		}
		return false;
	}

	void ModelPicker (int i)
	{
		GameObject model = (GameObject)EditorGUILayout.ObjectField ("Model:", nodeDTCur.triggersList [i].model, typeof(GameObject), false);

		// Prefab path
		if (model != nodeDTCur.triggersList [i].model)
		{
			nodeDTCur.triggersList [i].model = model;

			nodeDTCur.triggersList [i].prefab = AssetDatabase.GetAssetPath (model);
			nodeDTCur.triggersList [i].prefab = nodeDTCur.triggersList [i].prefab.Substring (0, nodeDTCur.triggersList [i].prefab.Length - 7);
			nodeDTCur.triggersList [i].prefab = nodeDTCur.triggersList [i].prefab.Substring (17);

			SetModel (i);
		}

		if (nodeDTCur.triggersList [i].model != null)
		{
			TriggerRotationUIControl (i);
		}

	}

	void RemoveTrigger (int n)
	{
		RemoveTriggerFromNode (nodeDTCur.triggersList [n].name);
		nodeDTCur.triggersList.RemoveAt (n);
	}

	void RemoveTriggerFromNode (string _name)
	{
		nodeDTCur.triggers.Remove (_name);
	}

	void AddTriggerToNode (string _name)
	{
		nodeDTCur.triggers.Add (_name);
	}

	void RemoveChild ()
	{
		while (nodeDTTrans.childCount > 0) {
			GameObject.DestroyImmediate (nodeDTTrans.GetChild (0).gameObject);
		}
	}

	void SetModel (int i)
	{
		// Remove old children
		RemoveChild ();

		// Instant new children
		GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab (nodeDTCur.triggersList [i].model);
		Transform newChildTransform = newChild.GetComponent<Transform> ();
		newChildTransform.SetParent (nodeDTTrans);
		newChildTransform.localPosition = Vector3.zero;
		newChildTransform.eulerAngles = MapController.GetEulerAngle (nodeDTCur.triggersList [i].modelDirection);
		newChildTransform.tag = "Untagged";
	}

	void TriggerRotationUIControl (int i)
	{
		Direction rotation = (Direction)EditorGUILayout.EnumPopup ("Model Rotation:", nodeDTCur.triggersList [i].modelDirection);

		if (rotation != nodeDTCur.triggersList [i].modelDirection)
		{
			nodeDTCur.triggersList [i].modelDirection = rotation;
			SetModel (i);
		}
	}

	void Destroy ()
	{
		nodeDTCur.triggers = new List<string> ();
		RemoveChild ();
	}

	void Create ()
	{
		GUILayout.BeginHorizontal (EditorStyles.helpBox);
		if (newTriggerName == "") {
			newTriggerName = "Trigger #" + nodeDTCur.triggersList.Count;
		}
		newTriggerName = EditorGUILayout.TextField ("New trigger name: ", newTriggerName);
		if (GUILayout.Button ("Create")) {CreateButton ();}
		GUILayout.EndHorizontal();
	}

	void CreateButton()
	{
		nodeDTCur.triggersList.Add (new TriggerDT ());
		nodeDTCur.triggersList[nodeDTCur.triggersList.Count - 1].name = newTriggerName;

		newTriggerName = "";
	}

	void UpdateTriggersList ()
	{
		// Populate names
		List<string> triggersNames = new List<string>();
		for (int j = 0; j < nodeDTCur.triggersList.Count; j++)
		{
			triggersNames.Add (nodeDTCur.triggersList [j].name);
		}

		// Send current TriggersList to everyone
		for (int i = 0; i < nodes.Length; i++)
		{
			nodes[i].GetComponent<NodeDT>().triggersList = nodeDTCur.triggersList;
			for (int k = 0; k < nodeDTCur.triggers.Count; k++)
			{
				if (!triggersNames.Contains(nodeDTCur.triggers[k])) 
				{
					RemoveTriggerFromNode(nodeDTCur.triggers[k]);
					k--;
				}
			}
		}
	}
}
