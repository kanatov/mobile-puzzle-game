using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DynamicObject {

	[System.Serializable]
	public class PathIndexer {
		[SerializeField] List<int> path;

		public PathIndexer (List<Waypoint> _path) {
			List<int> intPath = new List<int>();
			foreach (var _waypoint in _path) {
				intPath.Add (_waypoint.id);
			}
			path = intPath;
		}

		public Waypoint this [int i] {
			get {
				return MapController.waypoints [path [i]];
			}
			set {
				path [i] = value.id;
			}
		}

		public int Count {
			get {
				return path.Count;
			}
		}

		public int IndexOf (Waypoint _waypoint) {
			return path.IndexOf (_waypoint.id);
		}

		public void RemoveAt (int _index) {
			path.RemoveAt (_index);
		}
	}

	public int PositionInPath {
		get {
			return currentWaypoint;
		}
		set {
			currentWaypoint = value;

			if (currentWaypoint == Path.Count) {
				currentWaypoint = 0;
			}

			if (currentWaypoint == -1) {
				currentWaypoint = Path.Count - 1;
			}
		}
	}

	public PathIndexer Path;
	public int id;
	public string prefab;

	[SerializeField] protected int currentWaypoint;

	[System.NonSerialized] public GameObject model;

	public virtual void Move(){}
}