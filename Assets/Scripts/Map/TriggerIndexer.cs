using UnityEngine;
using System.Collections;

[System.Serializable]
public class TriggersIndexer {
	[SerializeField] int[] triggers;

	// Indexer
	public TriggersIndexer (int[] _triggers) {
		triggers = _triggers;
	}

	// Public Method
	public Trigger this [int i] {
		get {
			return MapController.triggers [triggers [i]];
		}
		set {
			triggers [i] = value.id;
		}
	}

	// Constructor
	public int Length {
		get {
			return triggers.Length;
		}
	}
}
