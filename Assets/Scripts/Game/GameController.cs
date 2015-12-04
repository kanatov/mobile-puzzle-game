using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public static class GameController {
	public static GameObject[] ui;
	static int animationsCounter;
	static InputField input;
	static bool firstTurn = true;
	public static bool enemyTurn = true;
	public static bool turnLock;


	public static void Init () {
		input = ui[0].GetComponent<InputField>();
		input.onEndEdit.AddListener(delegate{ChangeOverview(input);});
		LoadPlayerData();
		animationsCounter = 0;
		turnLock = false;

		// Create world
		Overview.Init();
		Map.Init ();
	}


	static void LoadPlayerData() {
		Player.x = PlayerDefault.x;
		Player.y = PlayerDefault.y;
		Player.health = PlayerDefault.maxHealth;
		Player.overview = PlayerDefault.overview;
	}


	public static int AnimationsCounter {
		set {
			animationsCounter += value;
			if (animationsCounter == 0){
				MakeTurn();
			} else {
				if (Player.character != null) {
					Player.character.GetComponent<PlayerInput>().enabled = false;
				}
			}
		}
	}


	static void MakeTurn() {
		if (Units.units.Count == 0) {
			Player.character.GetComponent<PlayerInput>().enabled = true;
		} else {
			Units.units.RemoveAt(0);
			MakeTurn();
		}
	}

	
	static void ChangeOverview (InputField _input) {
		Player.overview = int.Parse(_input.text);
		Map.Init ();
	}


	// Movement
	public static bool UnitInput (Cell _cell) {
		// Cell doe's not exist
		// Cell is obstackle
		// Cell contain a unit

		// Player:
		// 		said direction
		//			attack if possible
		//			walk if possible
		//		
		// Unit:
		//		said direction
		//			attack if possible
		//			change dirrection
		//		said direction
		//			walk if possible
		//			change dirrection

		if (_cell == null) {
			return false;
		}
		return false;
//		if (Player.x == cell.x && Player.y == cell.y) {
//			attack = true;
//			EnemyAttack (_unit);
//			break;
//		}
	}


	public static void InputLeft () {
		Map.UpdateMap(-1, 0);
	}
	public static void InputUp () {
		Map.UpdateMap(0, 1);
	}
	public static void InputRight () {
		Map.UpdateMap(1, 0);
	}

}
