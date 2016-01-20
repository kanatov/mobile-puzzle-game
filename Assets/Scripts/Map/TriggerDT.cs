using UnityEngine;
using System.Collections;

public class TriggerDT : MonoBehaviour {
	public GameObject[] activateWaypoints;
	public string prefab;
	public GameObject[] path;
	public bool removeOnActivation;
	[SerializeField]
	public GameObject model;
	public Direction tileDirection;
}
