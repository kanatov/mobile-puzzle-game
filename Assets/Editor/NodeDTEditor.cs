using UnityEngine;
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
		NodeTypes nodeType = (NodeTypes)EditorGUILayout.EnumPopup ("Neighbour Type:", nodeDT.type);

		if (nodeType != nodeDT.type) {
			nodeDT.type = nodeType;
		}

		if (nodeDT.type == NodeTypes.Ladder) {
			Direction ladderDirection = (Direction)EditorGUILayout.EnumPopup ("Ladder Base:", nodeDT.ladderDirection);

			if (ladderDirection != nodeDT.ladderDirection) {
				nodeDT.ladderDirection = ladderDirection;
			}
		}
	}
}
