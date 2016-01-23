﻿using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(NodeDT))]
public class NodeDTEditor : Editor {
	
	NodeDT nodeDT;
	Transform nodeDTTransform;

	public override void OnInspectorGUI() {
		nodeDT = (NodeDT) target;
		nodeDTTransform = nodeDT.GetComponent<Transform>();

		// Draw UI
		DefaultUIControl ("dynamicObjectTypes");
		UnitModelUIControl ();
		DefaultUIControl ("walkNodes");
		DefaultUIControl ("localNodes");
		DefaultUIControl ("triggers");
		DefaultUIControl ("singleActivation");
		DefaultUIControl ("touchActiovation");
		DefaultUIControl ("walk");
		NodeTypeUIControl ();

		// Save settings
		EditorSceneManager.MarkAllScenesDirty ();
		EditorUtility.SetDirty (nodeDT);
	}

	void UnitModelUIControl () {
		GameObject newModel = (GameObject)EditorGUILayout.ObjectField ("Collider:", nodeDT.model, typeof(GameObject), false);

		if (newModel != nodeDT.model) {
			nodeDT.model = newModel;

			if (nodeDT.model == null) {
				RemoveChild ();
				return;
			}
			SetModel ();
		}

		if (nodeDT.model != null) {
			DynamicObjectRotationUIControl ();
		}
	}

	void RemoveChild () {
		if (nodeDTTransform.childCount > 0) {
			GameObject.DestroyImmediate (nodeDTTransform.GetChild (0).gameObject);
		}
	}

	void SetModel () {
		RemoveChild ();

		// Instant new children
		GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab (nodeDT.model);
		Transform newChildTransform = newChild.GetComponent<Transform> ();
		newChildTransform.SetParent (nodeDTTransform);
		newChildTransform.localPosition = Vector3.zero;
		newChildTransform.eulerAngles = MapController.GetEulerAngle(nodeDT.unitDirection);
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

	void DynamicObjectRotationUIControl () {
		Direction rotation = (Direction)EditorGUILayout.EnumPopup ("Unit Rotation:", nodeDT.unitDirection);

		if (rotation != nodeDT.unitDirection) {
			nodeDT.unitDirection = rotation;
			SetModel ();
		}
	}

	void NodeTypeUIControl () {
		NodeTypes nodeType = (NodeTypes)EditorGUILayout.EnumPopup ("Neighbour Type:", nodeDT.Type);

		if (nodeType != nodeDT.Type) {
			nodeDT.Type = nodeType;
		}

		if (nodeDT.Type == NodeTypes.Ladder) {
			Direction ladderDirection = (Direction)EditorGUILayout.EnumPopup ("Ladder Base:", nodeDT.ladderDirection);

			if (ladderDirection != nodeDT.ladderDirection) {
				nodeDT.ladderDirection = ladderDirection;
			}
		}
	}
}
