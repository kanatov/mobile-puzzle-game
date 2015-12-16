using UnityEngine;
using System.Collections;

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
				new Trigger[waypointDT.triggers.Count]
			);
		}

		for (int i = 0; i < waypoints.Length; i++) {
			WaypointDT waypointDT = waypointsDT [i].GetComponent<WaypointDT> ();
			for (int k = 0; k < waypoints [i].neighbours.Length; k++) {
				waypoints [i].neighbours [k] = GetWaypoint (waypointDT.neighbours[k]);
			}

			for (int l = 0; l < waypoints [i].triggers.Length; l++) {
				waypoints [i].triggers [l] = GetTrigger (waypointDT.triggers[l]);
			}

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
}
