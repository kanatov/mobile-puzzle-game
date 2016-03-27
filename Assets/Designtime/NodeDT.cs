﻿using UnityEngine;
using System.Collections;

public class NodeDT : MonoBehaviour {
	// Cell properties
	public GameObject[] triggers;
	public GameObject[] walkNodes;
	public GameObject[] localNodes;
	public bool singleActivation;
	public bool touchActiovation;
	public bool walk;
	public NodeTypes type;
	public Direction ladderDirection;
}