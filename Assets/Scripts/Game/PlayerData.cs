using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LevelState {
	Locked = 0,
	Finished
}

[System.Serializable]
public class PlayerData {
	public List<LevelData> levelsData;
	public int currentLevel = 0;

	public PlayerData() {
		levelsData = new List<LevelData>();

		for (int i = 0; i < Application.levelCount; i++) {
			levelsData.Add(new LevelData(i));
		}
	}

	public void Update() {
		if (levelsData.Count != Application.levelCount) {
			List<LevelData> newLevelsData = new List<LevelData>();

			for (int i = 0; i < Application.levelCount; i++)
			{
				if (i < levelsData.Count) {
					newLevelsData.Add(levelsData[i]);
				} else {
					newLevelsData.Add(new LevelData(i));
				}
			}

			levelsData = newLevelsData;
		}
	}
}
