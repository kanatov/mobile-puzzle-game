﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDT : MonoBehaviour {
    public GameObject[] walkNodes;
    public GameObject[] localNodes;
    public bool singleActivation;
    public bool touchActiovation;
    public bool walk;
    public NodeTypes type;
    public Direction ladderDirection;

    public InteractiveTypes interactiveType = InteractiveTypes.None;

    public List<TriggerDT> triggersList;
    public List<string> triggers;

    public DynamicObjectTypes dynamicObjectType;
    public GameObject dynamicObjectModel;
    public string dynamicObjectPrefabPath;
    public Direction dynamicObjectDirection;
}