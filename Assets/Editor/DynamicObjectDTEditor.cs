using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(DynamicObjectDT))]
public class DynamicObjectDTEditor : Editor {
	
	DynamicObjectDT dynamicObjectDT;
	Transform dynamicObjectDTTransform;

	public override void OnInspectorGUI()
	{
		dynamicObjectDT = (DynamicObjectDT) target;
		dynamicObjectDTTransform = dynamicObjectDT.GetComponent<Transform>();

		// Draw UI
		UnitModelUIControl ();

		// Save settings
		EditorSceneManager.MarkAllScenesDirty ();
		EditorUtility.SetDirty (dynamicObjectDT);

		if(GUILayout.Button("Destroy dynamic object"))
		{
			Destroy();
		}
	}

	void UnitModelUIControl ()
	{
		// Collider type selector
		DynamicObjectTypes colliderType = (DynamicObjectTypes)EditorGUILayout.EnumPopup ("Dynamic Object Type:", dynamicObjectDT.dynamicObjectType);

		if (colliderType != dynamicObjectDT.dynamicObjectType) {
			dynamicObjectDT.dynamicObjectType = colliderType;
			SetModel ();
		}

		// Model selector
		GameObject newModel = (GameObject)EditorGUILayout.ObjectField ("Dynamic Object:", dynamicObjectDT.model, typeof(GameObject), false);

		if (newModel != dynamicObjectDT.model) {
			dynamicObjectDT.model = newModel;
			SetModel ();
		}
		DynamicObjectRotationUIControl ();
	}

	void RemoveChild ()
	{
		if (dynamicObjectDTTransform.childCount > 0) {
			GameObject.DestroyImmediate (dynamicObjectDTTransform.GetChild (0).gameObject);
		}
	}

	void SetModel ()
	{
		RemoveChild ();

		// Instant new children
		GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab (dynamicObjectDT.model);
		Transform newChildTransform = newChild.GetComponent<Transform> ();
		newChildTransform.SetParent (dynamicObjectDTTransform);
		newChildTransform.localPosition = Vector3.zero;
		newChildTransform.eulerAngles = MapController.GetEulerAngle(dynamicObjectDT.unitDirection);
		newChildTransform.tag = "Untagged";
	}

	void DynamicObjectRotationUIControl () {
		Direction rotation = (Direction)EditorGUILayout.EnumPopup ("Unit Rotation:", dynamicObjectDT.unitDirection);

		if (rotation != dynamicObjectDT.unitDirection) {
			dynamicObjectDT.unitDirection = rotation;
			SetModel ();
		}
	}

	void Destroy()
	{
		RemoveChild ();
		DestroyImmediate (dynamicObjectDT);
	}
}
