﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GenericData;

public enum UnitRotation {
	Forward = 0,
	ForwardRight,
	BackwardRight,
	Backward,
	BackwardLeft,
	ForwardLeft
}

public enum TriggersTypes {
	Button = 0,
	Lock
}

public enum TerrainsTypes {
	Grass = 0,
	Rock
}

public static class MapController {
	public static string TAG_UNIT = "Unit";
	public static string TAG_TILE = "Tile";
	public static string TAG_WAYPOINT = "Waypoint";
	public static string TAG_TRIGGER = "Trigger";
	public static string TAG_CONTAINER = "sContainer";

	public static GameObject waypointCollider = Resources.Load<GameObject>("Waypoint/Waypoint_Collider");
	public static Waypoint[] waypoints;
	public static Unit[] units;
	public static Trigger[] triggers;

	public static Unit player;

	static GameObject[] waypointsDT;

	public static void Init () {
		Debug.Log ("Map init");

		waypointsDT = GameObject.FindGameObjectsWithTag (TAG_WAYPOINT);

		waypoints = (Waypoint[])SaveLoad.Load (SaveLoad.nameGameSessionWaypoints);
		units = (Unit[])SaveLoad.Load (SaveLoad.nameGameSessionUnits);
		triggers = (Trigger[])SaveLoad.Load (SaveLoad.nameGameSessionTriggers);

		if (waypoints == null || units == null || triggers == null) {
			Debug.Log ("Map init: Prepare New Level: Waypoints == " + waypoints + ", Units == " + units + ", Triggers == " + triggers);
			PrepareNewLevel ();
		} else {
			Debug.Log ("Map init: Prepare Game Session");
			PrepareGameSession ();
		}

		// Remove references and objects
		waypointsDT = null;
		GameObject.DestroyImmediate (GameObject.FindGameObjectWithTag (TAG_TRIGGER + TAG_CONTAINER));
		GameObject.DestroyImmediate (GameObject.FindGameObjectWithTag (TAG_WAYPOINT + TAG_CONTAINER));

		// Organise scene
		SetContainer (TAG_WAYPOINT);
		SetContainer (TAG_TRIGGER);
		SetContainer (TAG_UNIT);
	}


	static void PrepareNewLevel () {
		waypoints = new Waypoint[waypointsDT.Length];
		List <Unit> newUnits = new List<Unit> ();
		List <Trigger> newTriggers = new List<Trigger> ();

		// Populate empty waypoints
		for (int i = 0; i < waypoints.Length; i++) {
			WaypointDT waypointDT = waypointsDT [i].GetComponent<WaypointDT> ();

			waypoints [i] = new Waypoint (
				waypointDT.walkable,
				waypointDT.GetComponent<Transform> ().position,
				new Waypoint[waypointDT.neighbours.Count],
				new Trigger[waypointDT.triggers.Count]
			);
		}

		// Fill waypoints
		for (int i = 0; i < waypoints.Length; i++) {
			WaypointDT waypointDT = waypointsDT [i].GetComponent<WaypointDT> ();

			// Prepare neighbours
			for (int k = 0; k < waypoints [i].neighbours.Length; k++)
				waypoints [i].neighbours [k] = GetWaypointByGO (waypointDT.neighbours[k]);

			// Prepare triggers
			for (int l = 0; l < waypoints [i].triggers.Length; l++) {
				waypoints [i].triggers [l] = GetTrigger (waypointDT.triggers [l]);
				newTriggers.Add (waypoints [i].triggers [l]);
			}
			
			// Prepare units
			if (waypointDT.unitPrefab != null && waypointDT.unitPrefab != "") {
				newUnits.Add(GetUnit (waypointDT));
			}
		}

		// Copy new dynamic objects
		units = new Unit[newUnits.Count];
		for (int i = 0; i < units.Length; i++) {
			units [i] = newUnits [i];
		}

		triggers = new Trigger[newTriggers.Count];
		for (int i = 0; i < triggers.Length; i++) {
			triggers [i] = newTriggers [i];
		}

		SaveLoad.Save (waypoints, SaveLoad.nameGameSessionWaypoints);
		SaveLoad.Save (units, SaveLoad.nameGameSessionUnits);
		SaveLoad.Save (triggers, SaveLoad.nameGameSessionTriggers);
	}


	static Trigger GetTrigger(GameObject _triggerDT) {
		TriggerDT triggerDT = _triggerDT.GetComponent<TriggerDT> ();
		Transform triggerDTTrans = _triggerDT.GetComponent<Transform> ();

		// Copy activateWaypoints
		Waypoint[] activateWaypoints = new Waypoint[triggerDT.activateWaypoints.Length];
		for (int i = 0; i < triggerDT.activateWaypoints.Length; i++) {
			activateWaypoints [i] = GetWaypointByGO (triggerDT.activateWaypoints[i]);
		}

		// Copy path
		List<Waypoint> path = new List<Waypoint> ();
		foreach(var _waypoint in triggerDT.path){
			path.Add (GetWaypointByGO (_waypoint));
		};

		Trigger trigger = new Trigger(
			path,
			triggerDT.prefab,
			0,
			activateWaypoints
		);
		return trigger;
	}


	static Unit GetUnit(WaypointDT _waypointDT) {
		Unit unit = new Unit (
			_waypointDT.unitPrefab,
			_waypointDT.unitRotation,
			GetWaypointByGO (_waypointDT.gameObject)
		);

		return unit;
	}


	static void PrepareGameSession () {
		foreach (var _waypoint in waypoints) {
			_waypoint.SetModel ();
		}
		foreach (var _unit in units) {
			_unit.SetModel ();
		}
		foreach (var _trigger in triggers) {
			_trigger.SetModel ();
		}
	}


	// Calculate new path
	public static List<Waypoint> GetPath (Waypoint _source, Waypoint _target) {
		if (_source == _target) {
			Debug.LogWarning ("Pathfinding: source == target");
			return null;
		}

		List<Waypoint> opened = new List<Waypoint> ();
		HashSet<Waypoint> closed = new HashSet<Waypoint> ();

		opened.Add (_source);

		while (opened.Count > 0) {
			// Assign some active node as current
			Waypoint currentWaypoint = opened [0];

			// Looking for the closest node to our target
			for (int i = 1; i < opened.Count; i++) {
				// If the fCost of some node is less then current cell fCost
				// or
				// If the fCost of some node is equal but hCost is less
				if (opened [i].fCost < currentWaypoint.fCost || opened [i].fCost == currentWaypoint.fCost && opened [i].hCost < currentWaypoint.hCost) {
					currentWaypoint = opened [i];
				}
			}

			// Closest node was found
			// Let's remove it from active node and put it to the closed
			opened.Remove (currentWaypoint);
			closed.Add (currentWaypoint);

			if (currentWaypoint == _target)
				break;

			// For every neighbour of current cell
			for (int i = 0; i < currentWaypoint.neighbours.Length; i++) {
				if (closed.Contains (currentWaypoint.neighbours [i]))
					continue;
				
				if (!currentWaypoint.neighbours [i].walkable)
					continue;

				float newMovementCostToNeghbour = currentWaypoint.gCost + Vector3.Distance (currentWaypoint.position, currentWaypoint.neighbours [i].position);

				if (newMovementCostToNeghbour < currentWaypoint.neighbours [i].gCost || !opened.Contains (currentWaypoint.neighbours [i])) {
					currentWaypoint.neighbours [i].gCost = newMovementCostToNeghbour;
					currentWaypoint.neighbours [i].hCost = Vector3.Distance (currentWaypoint.neighbours[i].position, _target.position);

					currentWaypoint.neighbours [i].parent = currentWaypoint;

					if (!opened.Contains (currentWaypoint.neighbours [i])) {
						opened.Add (currentWaypoint.neighbours [i]);
					}
				}
			}
		}

		if (!closed.Contains (_target)) {
			Waypoint closestCell = null;
			float distance = 0;

			foreach (var cell in closed) {
				float altDistance = Vector3.Distance(cell.position, _target.position);
				if (distance == 0 || altDistance < distance){
					closestCell = cell;
					distance = altDistance;
				}
			}

			_target = closestCell;
		}

		List<Waypoint> path = new List<Waypoint> ();
		Waypoint tmpCell = _target;

		while (tmpCell != _source) {
			path.Add (tmpCell);
			tmpCell = tmpCell.parent;
		}
		path.Add (_source);

		path.Reverse ();
		return path;
	}
		
	public static int GetRotationDegree (UnitRotation _unitRotation) {
		return (int)_unitRotation * 60;
	}

	public static Vector3 GetEulerAngle (UnitRotation _rotation) {
		return new Vector3 (0f, MapController.GetRotationDegree (_rotation), 0f);
	}

	// Helpers
	static Waypoint GetWaypointByGO (GameObject _target) {
		int i = System.Array.IndexOf (waypointsDT, _target);
		return waypoints [i];
	}

	public static Waypoint GetWaypointByV3 (List<Waypoint> _waypoints, GameObject _target) {
		int i = System.Array.IndexOf (waypointsDT, _target);
		return waypoints [i];
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
 }
