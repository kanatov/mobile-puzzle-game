using UnityEngine;
using System.Collections;

public enum LevelState {
	GameSession = 0,
	Completed,
	Locked
}

[System.Serializable]
public class PlayerData {
	public LevelState[] levelState;

	public PlayerData() {
		levelState = new LevelState[Application.levelCount];

		for (int i = 0; i < levelState.Length; i++) {
			levelState[i] = LevelState.Locked;
		}
	}

	public void Init() {
		if (levelState.Length != Application.levelCount) {
			LevelState[] newLevelState = new LevelState[Application.levelCount];

			for (int i = 0; i < newLevelState.Length; i++) {
				if (i < levelState.Length) {
					newLevelState [i] = levelState [i];
				} else {
					newLevelState [i] = LevelState.Locked;
				}
			}
		}
	}
}
