using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DynamicObject {

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
	public Direction modelDirection;
	public int id;
	public string prefabPath;
	[SerializeField] protected int currentNode;

	[System.NonSerialized] public GameObject model;

	public virtual void Move(string _callback){}
	public virtual void SetModel(){}
}