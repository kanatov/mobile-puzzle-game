using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TriggerDT {
	public GameObject[] activateNodes;
	public string prefab;
	public GameObject[] path;
	public bool removeOnActivation;
	[SerializeField] public GameObject model;
	public Direction modelDirection;
}
