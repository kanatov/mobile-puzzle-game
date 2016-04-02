using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDT : MonoBehaviour {
	// Cell properties
	public List<string> triggers;
	public GameObject[] walkNodes;
	public GameObject[] localNodes;
	public bool singleActivation;
	public bool touchActiovation;
	public bool walk;
	public NodeTypes type;
	public Direction ladderDirection;
	public List<TriggerDT> triggersList;
}