﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GenericData;
using System.Linq;

public enum Direction {
	Forward = 0,
	ForwardRight,
	BackwardRight,
	Backward,
	BackwardLeft,
	ForwardLeft
}

public enum NodeTypes {
	Horisontal = 0,
	Ladder,
	Vertical
}

public enum DynamicObjectTypes {
	Unit = 0,
	SnowBall
}
	
public static class MapController {
	public static float tileHeight = 0.5f;
	public static string TAG_UNIT = "Unit";
	public static string TAG_TILE = "Tile";
	public static string TAG_NODE = "Node";
	public static string TAG_TRIGGER = "Trigger";
	public static string TAG_CONTAINER = "sContainer";

	public static GameObject nodeCollider = Resources.Load<GameObject>("Nodes/NodeCollider");
	public static Node[] walkNodes;
	public static List<DynamicObject> dynamicObjects;
	public static Trigger[] triggers;
	public static Unit player;

	public static List<Trigger> triggersList;
	static GameObject[] nodesDT;
	static List<GameObject> triggersDT;

	public static void Init () {
		D.Log ("___Map init");
		GameController.ClearSavedData ();

		nodesDT = GameObject.FindGameObjectsWithTag (TAG_NODE);
		triggersDT = new List<GameObject> ();
		triggersList = new List<Trigger> ();

		GameController.LoadGameSession ();

		if (walkNodes == null || dynamicObjects == null || triggers == null) {
			D.Log ("___Map init: Prepare New Level");
			PrepareNewLevel ();
		} else {
			D.Log ("___Map init: Prepare Game Session");
			PrepareGameSession ();
		}

		// Remove references and objects
		nodesDT = null;
		triggersDT = null;
		triggersList = null;
		GameObject.DestroyImmediate (GameObject.FindGameObjectWithTag (TAG_TRIGGER + TAG_CONTAINER));
		GameObject.DestroyImmediate (GameObject.FindGameObjectWithTag (TAG_NODE + TAG_CONTAINER));

		// Organise scene
		SetContainer (TAG_NODE);
		SetContainer (TAG_TRIGGER);
		SetContainer (TAG_UNIT);
	}


	static void PrepareNewLevel () {
		walkNodes = new Node[nodesDT.Length];
		List <DynamicObject> newDynamicObjects = new List<DynamicObject> ();

		D.Log ("Map Init: Populate empty nodes");
		// Populate empty nodes
		for (int i = 0; i < walkNodes.Length; i++) {
			NodeDT nodeDT = nodesDT [i].GetComponent<NodeDT> ();
			walkNodes [i] = new Node (
				i,
				nodeDT.Type,
				nodeDT.walk,
				nodeDT.singleActivation,
				nodeDT.touchActiovation,
				nodeDT.GetComponent<Transform> ().position,
				new int[nodeDT.walkNodes.Count],
				new int[nodeDT.triggers.Count]
			);
		}

		D.Log ("Map Init: Fill nodes");
		// Fill nodes
		for (int i = 0; i < walkNodes.Length; i++) {
			NodeDT nodeDT = nodesDT [i].GetComponent<NodeDT> ();

			// Prepare neighbours
			for (int k = 0; k < walkNodes [i].WalkNodes.Length; k++) {
				walkNodes [i].WalkNodes [k] = GetNodeByGO (nodeDT.walkNodes [k]);
			}

			// Prepare triggers
			for (int l = 0; l < walkNodes [i].Triggers.Length; l++) {
				walkNodes [i].Triggers [l] = GetTrigger (nodeDT.triggers [l]);
			}

			// Prepare units
			if (nodeDT.colliderPrefabPath != null && nodeDT.colliderPrefabPath != "") {
				newDynamicObjects.Add (GetDynamicObject (nodeDT));
			}
		}

		D.Log ("Map Init: Copy dynamic objects");
		// Copy new dynamic objects
		dynamicObjects = newDynamicObjects;

		// Copy Trigger List to Array
		triggers = new Trigger[triggersList.Count];
		for (int i = 0; i < triggers.Length; i++) {
			triggers [i] = triggersList [i];
		}

		GameController.SaveGameSession ();
	}


	static Trigger GetTrigger(GameObject _triggerDT) {
		// Check for duplicate
		int triggerIndex = triggersDT.IndexOf (_triggerDT);
		if (triggerIndex != -1) {
			return triggersList [triggerIndex];
		}

		// Prepare new Trigger
		triggersDT.Add (_triggerDT);
		TriggerDT triggerDT = _triggerDT.GetComponent<TriggerDT> ();
//		Transform triggerDTTrans = _triggerDT.GetComponent<Transform> ();

		// Copy activateNodes
		int[] activateNodes = new int[triggerDT.activateNodes.Length];
		for (int i = 0; i < triggerDT.activateNodes.Length; i++) {
			activateNodes [i] = GetNodeByGO (triggerDT.activateNodes [i]).id;
		}

		// Copy path
		List<Node> path = new List<Node> ();
		foreach(var _node in triggerDT.path){
			path.Add (GetNodeByGO (_node));
		};

		triggersList.Add (new Trigger(
			triggersList.Count,
			path,
			triggerDT.prefab,
			triggerDT.tileDirection,
			0,
			activateNodes,
			triggerDT.removeOnActivation
		));
		return triggersList.Last();
	}


	static DynamicObject GetDynamicObject(NodeDT _nodeDT) {
		DynamicObject dynamicObject;

		switch (_nodeDT.dynamicObjectTypes) {
		case DynamicObjectTypes.SnowBall :
			dynamicObject = new Snowball (
				_nodeDT.colliderPrefabPath,
				GetNodeByGO (_nodeDT.gameObject)
			);
			break;

		default :
			dynamicObject = new Unit (
				_nodeDT.colliderPrefabPath,
				_nodeDT.unitDirection,
				GetNodeByGO (_nodeDT.gameObject)
			);
			break;
		}

		return dynamicObject;
	}


	static void PrepareGameSession () {
		foreach (var _node in walkNodes) {
			_node.SetModel (_node.Walk);
		}
		foreach (var _dynamicObject in dynamicObjects) {
			_dynamicObject.SetModel ();
		}
		foreach (var _trigger in triggers) {
			_trigger.SetModel ();
		}
	}

	public static void RotateCamera(SwipeDirection _swipeDir) {
		GameObject cameraContainer = GameObject.FindGameObjectWithTag ("CameraContainer");
		cameraContainer.GetComponent<Rotate1>().Target = _swipeDir;
		cameraContainer.GetComponent<Rotate1>().enabled = true;
	}

	// Pathfinding
	public static List<Node> GetPath (Node _source, Node _target) {
		if (_source == _target) {
			D.LogWarning ("Pathfinding: source == target");
			return null;
		}

		List<Node> opened = new List<Node> ();
		HashSet<Node> closed = new HashSet<Node> ();

		opened.Add (_source);

		while (opened.Count > 0) {
			// Assign some active node as current
			Node currentNode = opened [0];

			// Looking for the closest node to our target
			for (int i = 1; i < opened.Count; i++) {
				// If the fCost of some node is less then current cell fCost
				// or
				// If the fCost of some node is equal but hCost is less
				if (opened [i].fCost < currentNode.fCost || opened [i].fCost == currentNode.fCost && opened [i].hCost < currentNode.hCost) {
					currentNode = opened [i];
				}
			}

			// Closest node was found
			// Let's remove it from active node and put it to the closed
			opened.Remove (currentNode);
			closed.Add (currentNode);

			if (currentNode == _target)
				break;

			// For every neighbour of current cell
			for (int i = 0; i < currentNode.WalkNodes.Length; i++) {
				if (closed.Contains (currentNode.WalkNodes [i]))
					continue;
				
				if (!currentNode.WalkNodes [i].Walk)
					continue;

				float newMovementCostToNeghbour = currentNode.gCost + Vector3.Distance (currentNode.Position, currentNode.WalkNodes [i].Position);

				if (newMovementCostToNeghbour < currentNode.WalkNodes [i].gCost || !opened.Contains (currentNode.WalkNodes [i])) {
					currentNode.WalkNodes [i].gCost = newMovementCostToNeghbour;
					currentNode.WalkNodes [i].hCost = Vector3.Distance (currentNode.WalkNodes[i].Position, _target.Position);

					currentNode.WalkNodes [i].parent = currentNode;

					if (!opened.Contains (currentNode.WalkNodes [i])) {
						opened.Add (currentNode.WalkNodes [i]);
					}
				}
			}
		}

		if (!closed.Contains (_target)) {
			Node closestCell = null;
			float distance = 0;

			foreach (var cell in closed) {
				float altDistance = Vector3.Distance(cell.Position, _target.Position);
				if (distance == 0 || altDistance < distance){
					closestCell = cell;
					distance = altDistance;
				}
			}

			_target = closestCell;
		}

		List<Node> path = new List<Node> ();
		Node tmpCell = _target;

		while (tmpCell != _source) {
			path.Add (tmpCell);
			tmpCell = tmpCell.parent;
		}
		path.Add (_source);

		path.Reverse ();
		return path;
	}
		
	// Helpers
	public static int GetRotationDegree (Direction _unitRotation) {
		return (int)_unitRotation * 60;
	}

	public static Vector3 GetEulerAngle (Direction _rotation) {
		return new Vector3 (0f, GetRotationDegree (_rotation), 0f);
	}

	static Node GetNodeByGO (GameObject _target) {
		int i = System.Array.IndexOf (nodesDT, _target);
		return walkNodes [i];
	}

	// Scene clear
	public static GameObject[] SetContainer (string _tag) {
		GameObject container = GameObject.FindGameObjectWithTag (_tag + TAG_CONTAINER);
		
		if (container == null) {
			container = new GameObject ();
			container.GetComponent<Transform> ().position = Vector3.zero;
			string tag = _tag + TAG_CONTAINER;
			container.tag = tag;
			container.name = tag;
		}
		
		Transform containerTransform = container.GetComponent<Transform> ();
		
		GameObject[] items = GameObject.FindGameObjectsWithTag (_tag);
		
		foreach (var _item in items) {
			_item.GetComponent<Transform> ().SetParent (containerTransform);
		}
		
		return items;
	}

	public static Direction GetOppositeDirection (Direction _direction) {
		int directionNumber = (int)_direction;
		int directionsCount = System.Enum.GetValues (typeof(Direction)).Length;
		if (directionNumber < 3) {
			int oppositeDirection = directionNumber + directionsCount / 2;
			return (Direction)oppositeDirection;
		} else {
			int oppositeDirection = directionNumber - directionsCount / 2;
			return (Direction)oppositeDirection;
		}
	}

	public static Direction GetPointDirection (Vector3 _source, Vector3 _target) {
		Vector3 difference = _target - _source;

		float x = difference.x;
		float y = difference.z;

		if (y > 0) {
			if (x > 0) {
				return Direction.ForwardRight;
			}
			if (x < 0) {
				return Direction.ForwardLeft;
			}

			return Direction.Forward;
		} else {
			if (x > 0) {
				return Direction.ForwardRight;
			}
			if (x < 0) {
				return Direction.BackwardLeft;
			}

			return Direction.Backward;
		}
	}
}
