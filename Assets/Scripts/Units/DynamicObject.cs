using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DynamicObject {

	[System.Serializable]
	public class PathIndexer {
		[SerializeField] List<int> path;

		public PathIndexer (List<Node> _path) {
			List<int> intPath = new List<int>();
			foreach (var _node in _path) {
				intPath.Add (_node.id);
			}
			path = intPath;
		}

		public Node this [int i] {
			get {
				return MapController.walkNodes [path [i]];
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

		public int IndexOf (Node _node) {
			return path.IndexOf (_node.id);
		}

		public void RemoveAt (int _index) {
			path.RemoveAt (_index);
		}
	}

	public int PositionInPath {
		get {
			return currentNode;
		}
		set {
			currentNode = value;

			if (currentNode == Path.Count) {
				currentNode = 0;
			}

			if (currentNode == -1) {
				currentNode = Path.Count - 1;
			}
		}
	}

	public PathIndexer Path;
	public Direction modelRotation;
	public int id;
	public string prefabPath;
	[SerializeField] protected int currentNode;

	[System.NonSerialized] public GameObject model;

	public virtual void Move(string _callback){}
	public virtual void SetModel(){}
}