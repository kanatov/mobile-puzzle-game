using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDT : MonoBehaviour {
	// Cell properties
	public List<GameObject> triggers;
	public List<GameObject> walkNodes;
	public List<GameObject> localNodes;
	public bool singleActivation;
	public bool touchActiovation;
	public bool walk;
	public int type;
	public NodeTypes Type {
		get {
			return (NodeTypes)type;
		}
		set {
			type = (int)value;
		}
	}
	public Direction ladderDirection;

	// Collider properties
	public DynamicObjectTypes dynamicObjectTypes;
	public GameObject model;
	[HideInInspector] public string colliderPrefabPath;
	public Direction unitDirection;
}