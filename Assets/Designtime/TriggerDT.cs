using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TriggerDT {
	public string name;
	public TriggerTypes type;
	public List<GameObject> activateNodes;
	public string prefabPath;
	public List<GameObject> path;
	public GameObject model;
	public Direction modelDirection;
}
