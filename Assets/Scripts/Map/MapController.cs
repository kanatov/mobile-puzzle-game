using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TriggersTypes {
	Button = 0,
	Tile
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

	public static GameObject[] tilesModels = new GameObject[] {
		Resources.Load<GameObject>("Tiles/Tile_Grass"),
		Resources.Load<GameObject>("Tiles/Tile_Rock")
	};
	public static GameObject[] triggersModels = new GameObject[] {
		Resources.Load<GameObject>("Triggers/Trigger_Grass"),
		Resources.Load<GameObject>("Triggers/Trigger_Rock")
	};
	public static GameObject waypointCollider = Resources.Load<GameObject>("Waypoint/Waypoint_Collider");
	public static GameObject[] unitsModels = new GameObject[] {
		Resources.Load<GameObject>("Units/Unit_Friend"),
		Resources.Load<GameObject>("Units/Unit_Enemy")
	};
	static GameObject[] waypointsDT;
	static Waypoint[] waypoints;

	public static void Init () {
		PrepareWaypoints ();
	}

	static void PrepareWaypoints () {
		waypointsDT = GameObject.FindGameObjectsWithTag (TAG_WAYPOINT);
		waypoints = new Waypoint[waypointsDT.Length];

		for (int i = 0; i < waypoints.Length; i++) {
			WaypointDT waypointDT = waypointsDT [i].GetComponent<WaypointDT> ();

			waypoints [i] = new Waypoint (
				waypointDT.walkable,
				waypointDT.GetComponent<Transform> ().position,
				new Waypoint[waypointDT.neighbours.Count],
				new UnitRotation[waypointDT.rotations.Count],
				new Trigger[waypointDT.triggers.Count]
			);
		}

		for (int i = 0; i < waypoints.Length; i++) {
			WaypointDT waypointDT = waypointsDT [i].GetComponent<WaypointDT> ();
			for (int k = 0; k < waypoints [i].neighbours.Length; k++)
				waypoints [i].neighbours [k] = GetWaypoint (waypointDT.neighbours[k]);

			for (int l = 0; l < waypoints [i].triggers.Length; l++)
				waypoints [i].triggers [l] = GetTrigger (waypointDT.triggers[l]);

			for (int m = 0; m < waypoints [i].rotations.Length; m++)
				waypoints [i].rotations [m] = waypointDT.rotations[m];

			SetUnit (waypointDT);
		}

		waypointsDT = null;
		GameObject.DestroyImmediate (GameObject.FindGameObjectWithTag (TAG_TRIGGER + TAG_CONTAINER));
		GameObject.DestroyImmediate (GameObject.FindGameObjectWithTag (TAG_WAYPOINT + TAG_CONTAINER));

		GetContainer (TAG_WAYPOINT);
		GetContainer (TAG_TRIGGER);
		GetContainer (TAG_UNIT);
	}

	static Trigger GetTrigger(GameObject _triggerDT) {
		TriggerDT triggerDT = _triggerDT.GetComponent<TriggerDT> ();
		Trigger trigger = new Trigger (
			triggerDT.type,
			triggerDT.GetComponent<Transform>().position,
			triggerDT.state,
			new Waypoint[triggerDT.waypoints.Count]
		);

		for (int i = 0; i < trigger.waypoints.Length; i++) {
			trigger.waypoints [i] = GetWaypoint (triggerDT.waypoints[i]);
		}

		return trigger;
	}

	static Waypoint GetWaypoint (GameObject _target) {
		int i = System.Array.IndexOf (waypointsDT, _target);
		return waypoints [i];
	}

	public static GameObject[] GetContainer (string _tag) {
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

	static void SetUnit(WaypointDT _waypointDT) {
		if (_waypointDT.unitType == UnitsTypes.None) {
			return;
		}

		Waypoint[] unitWaypoints = new Waypoint[_waypointDT.unitWaypoints.Length];
		for (int i = 0; i < _waypointDT.unitWaypoints.Length; i++) {
			unitWaypoints [i] = GetWaypoint (_waypointDT.unitWaypoints [i].gameObject);
		}

		Unit unit = new Unit (
			_waypointDT.unitType,
			_waypointDT.unitRotation,
			unitWaypoints
		);
	}

	// Calculate new path
	public static List<Waypoint> FindPath (Waypoint _source, Waypoint _target) {
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
 }
