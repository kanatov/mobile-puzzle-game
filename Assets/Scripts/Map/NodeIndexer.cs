using UnityEngine;
using System.Collections;

[System.Serializable]
public class NodeIndexer {
	[SerializeField] int[] nodesID;

	// Indexer
	public Node this [int i] {
		get {
			if (nodesID == null || nodesID.Length == 0 || nodesID [i] == -1) {
				return null;
			} else {
				return MapController.currentLevelNodes [nodesID [i]];
			}
		}
		set {
			if (value == null) {
				nodesID [i] = -1;
			} else {
				nodesID [i] = value.id;
			}
		}
	}

	// Public Method
	public int Length {
		get {
			return nodesID.Length;
		}
	}

	// Constructor
	public NodeIndexer (int[] _neighbours) {
		nodesID = _neighbours;
	}
}
