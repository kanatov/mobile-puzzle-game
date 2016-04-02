using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TriggerDT {
	public string name;
	public List<GameObject> activateNodes;
	public string prefab;
	public List<GameObject> path;
	public bool removeOnActivation = false;
	public GameObject model;
	public Direction modelDirection;
}
