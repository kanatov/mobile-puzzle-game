using GenericData;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class GameController {
	// Prefabs
	public static GameObject camera;
	public static GameObject cellContainer;
	public static GameObject mapContainer;
	public static GameObject unitContainer;
	public static GameObject[] terrainModels;
	public static GameObject ui;
	public static GameObject uiMenu;
	public static GameObject uiMap;

	// Runtime data
	public static PlayerData playerData;
	static int animationsCounter;
	static InputField input;
	static bool firstTurn = true;

	public static void Init () {
		// Prepare prefabs

		camera = GameObject.FindGameObjectWithTag("MainCamera");
		cellContainer = Resources.Load<GameObject>("Prefabs/Containers/Cell");
		mapContainer = Resources.Load<GameObject>("Prefabs/Containers/Map");
		unitContainer = Resources.Load<GameObject>("Prefabs/Containers/Unit");

		terrainModels = new GameObject[] {
			Resources.Load<GameObject>("Prefabs/Models/Terrain/Grass"),
			Resources.Load<GameObject>("Prefabs/Models/Terrain/Rock")
		};

		// Prepare UI
		ui = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI"));
		uiMenu = ui.GetComponent<Transform>().FindChild("Menu").gameObject;
		uiMenu.SetActive(false);
		uiMap = ui.GetComponent<Transform>().FindChild("Map").gameObject;
		uiMap.SetActive(false);

		MyDebug.ClearData();
		LoadData();
		MakeTurn();
	}

	static void LoadData () {
		Debug.Log ("Loading Player data");
		playerData = (PlayerData)SaveLoad.Load(SaveLoad.namePlayerProgress);
		if (playerData == null) {
			Debug.Log ("First start");
			playerData = new PlayerData();
			SaveLoad.Save(playerData, SaveLoad.namePlayerProgress);
			Map.Init();
		} else {
			Debug.Log ("Loading Map");
			Map.currentLevel = (Cell[,,])SaveLoad.Load(SaveLoad.nameGame);
			Map.currentUnits = (List<Unit>)SaveLoad.Load(SaveLoad.nameUnits);

			if (Map.currentLevel == null || Map.currentUnits == null) {
				Debug.Log ("Loading Menu");
				UI.Init();
			} else {
				Debug.Log ("Continuing the Game");
				Map.Init();
			}
		}
	}

//	public static int AnimationsCounter {
//		set {
//			animationsCounter += value;
//			if (animationsCounter == 0){
//				MakeTurn();
//			} else {
//				if (Player.character != null) {
//					Player.character.GetComponent<PlayerInput>().enabled = false;
//				}
//			}
//		}
//	}
//
//
	static void MakeTurn() {
//		if (GameController == 0) {
//		Map.player.unitContainer.GetComponent<PlayerInput>().enabled = true;
//		SaveLoad.Save(Map.currentGame, SaveLoad.nameGame);
//		SaveLoad.Save(Map.currentUnits, SaveLoad.nameUnits);
//			return;
//		}
//		Units.EnemyBehaviour(Units.units[0]);
//		Units.units.RemoveAt(0);
	}
//
//	
//	static void ChangeOverview (InputField _input) {
//		Player.overview = int.Parse(_input.text);
//		Map.Init ();
//	}
//
//
//	// Movement
//	public static bool UnitInput (Cell _cell) {
//		if (_cell == null) {
//			return false;
//		}
//		return false;
//		if (Player.x == cell.x && Player.y == cell.y) {
//			attack = true;
//			EnemyAttack (_unit);
//			break;
//		}
//	}
}
