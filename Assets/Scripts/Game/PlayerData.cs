using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerData {
	public static Unit unit;
	public static int overview;
	public static float maxHealth;
	public static float damage;
	public static int playerLevel;

	public PlayerData () {
		playerLevel = 0;
		overview = 1;
		maxHealth = 10;
		damage = 2;
	}
	
}