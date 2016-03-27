using UnityEngine;
using System.Collections;

public class DynamicObjectDT : MonoBehaviour {
	// Collider properties
	public DynamicObjectTypes dynamicObjectType;
	public GameObject model;
	[HideInInspector] public string unitPrefabPath;
	public Direction unitDirection;
}