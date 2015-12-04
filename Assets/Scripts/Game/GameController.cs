using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public static class GameController {
	public static GameObject[] ui;
	static int turnLockQueue = 0;
	static InputField input;
	static bool firstTurn = true;
	public static bool enemyTurn = true;


	public static void Init () {
		input = ui[0].GetComponent<InputField>();
		input.onEndEdit.AddListener(delegate{ChangeOverview(input);});
		LoadPlayerData();

		// Create world
		Overview.Init();
		Map.Init ();
	}


	static void LoadPlayerData() {
		Player.x = PlayerDefault.x;
		Player.y = PlayerDefault.y;
		Player.overview = PlayerDefault.overview;
	}


	public static int TurnLock {
		get {
			return turnLockQueue;
		}
		set {
			turnLockQueue += value;

			if (turnLockQueue == 0 && enemyTurn){
				if (firstTurn) {
					firstTurn = false;
					return;
				}

				enemyTurn = !enemyTurn;
				EnemyTurn();
			}
		}
	}

	static void EnemyTurn () {
		foreach (var unitObject in Units.enemies) {
			Units.EnemyBehaviour(unitObject.GetComponent<Unit>());
		}
	}

	static void ChangeOverview (InputField _input) {
		Player.overview = int.Parse(_input.text);
		Map.Init ();
	}
}
