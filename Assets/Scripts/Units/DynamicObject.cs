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

			if (currentNode == path.Count) {
				currentNode = 0;
			}

			if (currentNode == -1) {
				currentNode = path.Count - 1;
			}
		}
	}

	public PathIndexer path;
	public Direction modelDirection;
	public int id;
    public string prefabPath;
    public bool tutorialTrigger;
    public bool tutorialActivated;

	[SerializeField] protected int currentNode;

	[System.NonSerialized] public GameObject model;

	public virtual void Move(string _callback){}
	public virtual void SetModel(){}
}