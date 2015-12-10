using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GenericData;
using UnityEngine.UI;

public static class Map {
	public static Cell[,] currentGame;
	public static List<Unit> currentUnits;
	static GameObject map;
	public static Unit player;

	// World Settings
	static float hexOffsetX, hexOffsetY;


	public static void Init () {
		Debug.Log ("Map.Init()");

		PrepareUI ();
		PrepareMapContainer ();
		
		if (currentGame == null || currentUnits == null) {
			currentGame = GetCells (PlayerData.currentLevel);
			PopulateNeighbours ();
		}

		PopulateCellModels ();
		PopulateUnits ();
	}


	static void PrepareUI () {
		GameController.uiMap.SetActive (true);
		Button menuButton = GameController.uiMap.GetComponent<Transform> ().FindChild ("Menu").GetComponent<Button> ();
		menuButton.onClick.AddListener (() =>  {
			UI.Init ();
		});
	}

	static void PrepareMapContainer () {
		map = GameObject.Instantiate (GameController.mapContainer);
		map.GetComponent<Transform> ().position = Vector3.zero;
	}


	static Cell[,] GetCells (int _playerLevel) {
		Debug.Log ("Map.GetCells()");
		int levelW = Levels.unitsAndItems[_playerLevel].GetLength(0);
		int levelH = Levels.unitsAndItems[_playerLevel].GetLength(1);
		Cell[,] newCells = new Cell[levelW,levelH];
		currentUnits = new List<Unit>();

		for (int x = 0; x < Levels.unitsAndItems[_playerLevel].GetLength(0); x++ ) {
			for (int y = 0; y < Levels.unitsAndItems[_playerLevel].GetLength(1); y++ ) {
				newCells[x, y] = new Cell();
				newCells[x, y].unitsAndItems = Levels.unitsAndItems[_playerLevel][x, y];
				newCells[x, y].x = x;
				newCells[x, y].y = y;

				// remove units in list from the map
				if (newCells[x, y].unitsAndItems < 0) {
					continue;
				}

				if (newCells[x, y].unitsAndItems < 16) {
					Unit unit = Units.GetUnit(newCells[x, y]);
					currentUnits.Add(unit);
					newCells[x, y].unitsAndItems = 0;
				}
			}
		}
		return newCells;
	}


	static void PopulateNeighbours () {
		Debug.Log ("Map.PopulateNeighbours()");
		// Add and copy cells to new array
		for (int x = 0; x < currentGame.GetLength(0); x++) {
			for (int y = 0; y < currentGame.GetLength(1); y++) {
				for (int n = 0; n < currentGame[x, y].neighbours.Length; n++) {
					currentGame[x, y].neighbours[n] = GetNeighbour(x, y, (Direction)n);
				}
			}
		}
	}


	static Cell GetNeighbour (int _x, int _y, Direction _n) {
		int newX = 0;
		int newY = 0;

		switch (_n) {
		case Direction.UpLeft:
			newX = _x - 1;
			newY = _y + GetShift(_x, 1, 0);
			break;
		case Direction.Up:
			newX = _x;
			newY = _y + 1;
			break;
		case Direction.UpRight:
			newX = _x + 1;
			newY = _y + GetShift(_x, 1, 0);
			break;
		case Direction.DownRight:
			newX = _x + 1;
			newY = _y - GetShift(_x, 1, 1);
			break;
		case Direction.Down:
			newX = _x;
			newY = _y - 1;
			break;
		case Direction.DownLeft:
			newX = _x - 1;
			newY = _y - GetShift(_x, 1, 1);
			break;
		}

		if (newX < 0 || newY < 0 || newX >= currentGame.GetLength(0) || newY >= currentGame.GetLength(1)) {
			return null;
		}
		if (Levels.terrains[PlayerData.currentLevel][newX, newY] == 0) {
			return currentGame[newX, newY];
		}
		return null;
	}


	static void PopulateCellModels () {
		Debug.Log ("Map.PopulateCellModels()");

		float d = 0.5f / (Mathf.Sqrt(3)/2);
		hexOffsetX = d * 1.5f;
		hexOffsetY = d * Mathf.Sqrt(3);

		// Add and copy cells to new array
		for (int x = 0; x < currentGame.GetLength(0); x++) {
			for (int y = 0; y < currentGame.GetLength(1); y++) {
				int terrain = Levels.terrains[PlayerData.currentLevel][x, y];
				currentGame[x, y].terrainModel = GetCellModel(terrain, x, y);
				if (currentGame[x, y].terrainModel != null) {
					currentGame[x, y].terrainModel.GetComponent<Highlite>().neighbours = currentGame[x, y].neighbours;
				}
			}
		}
	}


	static GameObject GetCellModel (int _terrain, int _x, int _y) {
		if (_terrain == -1) {
			return null;
		}
		GameObject cellModel = GameObject.Instantiate(GameController.terrainModels[_terrain]);
		Transform cellModelTransform = cellModel.GetComponent<Transform> ();
		cellModelTransform.SetParent (map.GetComponent<Transform>());
		cellModelTransform.localPosition = GetWorldPosition (_x, _y);
		return cellModel;
	}


	static void PopulateUnits () {
		Debug.Log ("Map.PopulateUnits()");
		
		foreach (var _unit in currentUnits) {
			// HACK
			_unit.cell = currentGame[_unit.cell.x, _unit.cell.y];

			_unit.unitContainer = Units.GetUnitContainer(_unit);
			Transform unitTransform = _unit.unitContainer.GetComponent<Transform> ();
			unitTransform.position = _unit.cell.terrainModel.GetComponent<Transform>().position;
			
			if (_unit.id == 0) {
				PreparePlayer (_unit);
			}
		}
	}
	
	static void PreparePlayer (Unit _unit) {
		player = _unit;
		Transform cameraTransform = GameController.camera.GetComponent<Transform>();
		cameraTransform.SetParent(_unit.unitContainer.GetComponent<Transform>());
		cameraTransform.localPosition = new Vector3 (0f, 7f, -5f);
		cameraTransform.localEulerAngles = new Vector3(35f, 0f, 0f);
	}


	static Vector3 GetWorldPosition (int _x, int _y) {
		float x, y;
		if(GetShift(_x, 1, 0) == 0) {
			x = _x * hexOffsetX;
			y = _y * hexOffsetY;
		} else {
			x = _x * hexOffsetX;
			y = (_y + 0.5f) * hexOffsetY;
		}

		return new Vector3 (x, 0f, y);
	}


	static int GetShift (int _x, int _shift, int _s) {
		if ((_x + _s) % 2 == 0) {
			return _shift;
		} else {
			return 0;
		}
	}
}
