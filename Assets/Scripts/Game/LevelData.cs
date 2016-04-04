using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelData {
	public int id;
	public LevelState state = LevelState.Locked;
	public Node[] currentLevelNodes;
	public List<DynamicObject> dynamicObjects;
	public Trigger[] triggers;

	public LevelData(int _id) {
		id = _id;
	}

	public void Reset()
	{
		currentLevelNodes = null;
		dynamicObjects = null;
		triggers = null;
	}
}
