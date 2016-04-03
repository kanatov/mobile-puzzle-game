using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PathIndexer {
	[SerializeField] List<int> path;

	public PathIndexer (List<Node> _path) {
		D.Log ("Init list: " + _path.Count);
		List<int> intPath = new List<int>();
		foreach (var _node in _path) {
			intPath.Add (_node.id);
		}
		path = intPath;
	}

	public Node this [int i] {
		get {
			if (path == null || path.Count == 0 || path [i] == -1) {
				return null;
			} else {
				return MapController.currentLevelNodes [path [i]];
			}
		}
		set {
			if (value == null) {
				path [i] = -1;
			} else {
				path [i] = value.id;
			}
		}
	}

	public int Count {
		get {
			return path.Count;
		}
	}

	public int IndexOf (Node _node) {
		return path.IndexOf (_node.id);
	}

	public void RemoveAt (int _index) {
		path.RemoveAt (_index);
	}
}