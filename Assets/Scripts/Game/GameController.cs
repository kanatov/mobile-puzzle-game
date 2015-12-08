using GenericData;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class GameController {
	// Prefabs
	public static GameObject cellContainer;
	public static GameObject mapContainer;
	public static GameObject unitContainer;
	public static GameObject[] terrainModels;
	public static UnitType[] unitTypes;
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


		LoadData();
	}

	static void LoadData () {
		Debug.Log ("Loading data");
		playerData = (PlayerData)SaveLoad.Load(SaveLoad.playerDataFileName);
		if (playerData == null) {
			Debug.Log ("Player data == null");
			playerData = new PlayerData();
			SaveLoad.Save(playerData, SaveLoad.playerDataFileName);
			Debug.Log("Player data save check: " + SaveLoad.Load(SaveLoad.playerDataFileName));
			Map.Init();
		} else {
			Debug.Log ("Player data loaded ");
			Map.currentLevel = (Cell[,])SaveLoad.Load(SaveLoad.levelDataFileName);
			if (Map.currentLevel == null) {
				Debug.Log ("Current level data == null");
				UI.Init();
			} else {
				Debug.Log ("Current level data loaded");
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
//	static void MakeTurn() {
//		if (Units.units.Count == 0) {
//			Player.character.GetComponent<PlayerInput>().enabled = true;
//			return;
//		}
//		Units.EnemyBehaviour(Units.units[0]);
//		Units.units.RemoveAt(0);
//	}
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
