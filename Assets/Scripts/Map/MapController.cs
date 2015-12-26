using UnityEngine;
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

public enum WaypointsTypes {
	Horisontal = 0,
	Ladder,
	Vertical
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
	public static float tileHeight = 0.5f;
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
		Debug.Log ("___Map init");

		waypointsDT = GameObject.FindGameObjectsWithTag (TAG_WAYPOINT);

		GameController.LoadGameSession ();

		if (waypoints == null || units == null || triggers == null) {
			Debug.Log ("___Map init: Prepare New Level");
			PrepareNewLevel ();
		} else {
			Debug.Log ("___Map init: Prepare Game Session");
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

		Debug.Log ("Map Init: Populate empty waypoints");
		// Populate empty waypoints
		for (int i = 0; i < waypoints.Length; i++) {
			WaypointDT waypointDT = waypointsDT [i].GetComponent<WaypointDT> ();

			waypoints [i] = new Waypoint (
				i,
				waypointDT.Type,
				waypointDT.walkable,
				waypointDT.GetComponent<Transform> ().position,
				new int[waypointDT.neighbours.Count],
				new int[waypointDT.triggers.Count]
			);
		}

		Debug.Log ("Map Init: Fill waypoints");
		// Fill waypoints
		for (int i = 0; i < waypoints.Length; i++) {
			WaypointDT waypointDT = waypointsDT [i].GetComponent<WaypointDT> ();

			// Prepare neighbours
			for (int k = 0; k < waypoints [i].Neighbours.Length; k++) {
				waypoints [i].Neighbours [k] = GetWaypointByGO (waypointDT.neighbours [k]);
			}

			// Prepare triggers
			for (int l = 0; l < waypoints [i].Triggers.Length; l++) {
				newTriggers.Add (GetTrigger (waypointDT.triggers [l]));
				waypoints [i].Triggers [l] = newTriggers.Last();
			}

			// Prepare units
			if (waypointDT.unitPrefab != null && waypointDT.unitPrefab != "") {

				newUnits.Add(GetUnit (waypointDT));
			}
		}

		Debug.Log ("Map Init: Copy dynamic objects");
		// Copy new dynamic objects
		units = new Unit[newUnits.Count];
		for (int i = 0; i < units.Length; i++) {
			units [i] = newUnits [i];
		}

		triggers = new Trigger[newTriggers.Count];
		for (int i = 0; i < triggers.Length; i++) {
			triggers [i] = newTriggers [i];
		}

		GameController.SaveGameSession ();
	}


	static Trigger GetTrigger(GameObject _triggerDT) {
		TriggerDT triggerDT = _triggerDT.GetComponent<TriggerDT> ();
		Transform triggerDTTrans = _triggerDT.GetComponent<Transform> ();
		// Copy activateWaypoints
		int[] activateWaypoints = new int[triggerDT.activateWaypoints.Length];
		for (int i = 0; i < triggerDT.activateWaypoints.Length; i++) {
			activateWaypoints [i] = GetWaypointByGO (triggerDT.activateWaypoints [i]).id;
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
			_waypointDT.unitDirection,
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
			for (int i = 0; i < currentWaypoint.Neighbours.Length; i++) {
				if (closed.Contains (currentWaypoint.Neighbours [i]))
					continue;
				
				if (!currentWaypoint.Neighbours [i].walkable)
					continue;

				float newMovementCostToNeghbour = currentWaypoint.gCost + Vector3.Distance (currentWaypoint.Position, currentWaypoint.Neighbours [i].Position);

				if (newMovementCostToNeghbour < currentWaypoint.Neighbours [i].gCost || !opened.Contains (currentWaypoint.Neighbours [i])) {
					currentWaypoint.Neighbours [i].gCost = newMovementCostToNeghbour;
					currentWaypoint.Neighbours [i].hCost = Vector3.Distance (currentWaypoint.Neighbours[i].Position, _target.Position);

					currentWaypoint.Neighbours [i].parent = currentWaypoint;

					if (!opened.Contains (currentWaypoint.Neighbours [i])) {
						opened.Add (currentWaypoint.Neighbours [i]);
					}
				}
			}
		}

		if (!closed.Contains (_target)) {
			Waypoint closestCell = null;
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
		
	public static int GetRotationDegree (Direction _unitRotation) {
		return (int)_unitRotation * 60;
	}

	public static Vector3 GetEulerAngle (Direction _rotation) {
		return new Vector3 (0f, GetRotationDegree (_rotation), 0f);
	}

	// Helpers
	static Waypoint GetWaypointByGO (GameObject _target) {
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
