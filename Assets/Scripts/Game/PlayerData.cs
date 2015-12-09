using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerData {
	public static int overview;
	public static int currentLevel;

	public PlayerData () {
		currentLevel = 0;
		overview = 1;
	}
	
}