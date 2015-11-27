using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class GameManager {
	public static GameObject[] UI;
	static int turn = 0;

	public static void Init (int[] _level, int[] _terrain) {
		MapManager.Create (_level, _terrain);

		turn = _level[4];
		UI[2].GetComponent<Text>().text = turn.ToString();
	}

	public static void MakeTurn () {
		turn--;
		if (turn <= 0) {
			Debug.Log (000);
		}
	}
}
