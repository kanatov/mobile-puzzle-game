using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GenericData;
using UnityEngine.UI;

public static class Map {
	public static Cell[,,] currentLevel;
	public static List<Unit> currentUnits;
	static GameObject map;
	public static Unit player;

	delegate void LoopFunction (int _x, int _y, int _z);
	static LoopFunction loopFunction;

	// World Settings
	static float hexOffsetX, hexOffsetY;


	public static void Init () {
		Debug.Log ("Map.Init()");

		PrepareUI ();
		PrepareMapContainer ();
		
		if (currentLevel == null || currentUnits == null) {
			PopulateCells ();
		}

		float d = 0.5f / (Mathf.Sqrt(3)/2);
		hexOffsetX = d * 1.5f;
		hexOffsetY = d * Mathf.Sqrt(3);

		loopFunction = PopulateCellModels;
		CurrentLevelLoop (loopFunction);

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

		Transform cameraTransform = GameController.camera.GetComponent<Transform>();
		cameraTransform.SetParent(map.GetComponent<Transform>());
		cameraTransform.localPosition = new Vector3 (-10f, 9.5f, -0.5f);
		cameraTransform.localEulerAngles = new Vector3(35f, 60f, 0f);
	}


	static void PopulateCells () {
		Debug.Log ("Map.GetCells()");
		int levelZ = Levels.terrains[PlayerData.currentGame].GetLength(0);
		int levelX = Levels.terrains[PlayerData.currentGame].GetLength(1);
		int levelY = Levels.terrains[PlayerData.currentGame].GetLength(2);

		currentLevel = new Cell[levelZ,levelX,levelY];
		currentUnits = new List<Unit>();

		loopFunction = PopulateCell;
		CurrentLevelLoop (loopFunction);

		loopFunction = PopulateNeighbours;
		CurrentLevelLoop (loopFunction);
	}


	static void PopulateCell (int _x, int _y, int _z) {
		currentLevel[_z, _x, _y] = new Cell();
		currentLevel[_z, _x, _y].unitsAndItems = Levels.unitsAndItems[PlayerData.currentGame][_z, _x, _y];
		currentLevel[_z, _x, _y].x = _x;
		currentLevel[_z, _x, _y].y = _y;
		currentLevel[_z, _x, _y].z = _z;
		
		// remove units in list from the map
		if (currentLevel[_z, _x, _y].unitsAndItems >= 0) {
			if (currentLevel[_z, _x, _y].unitsAndItems < 16) {
				Unit unit = Units.GetUnit(currentLevel[_z, _x, _y]);
				currentUnits.Add(unit);
				currentLevel[_z, _x, _y].unitsAndItems = 0;
			}
		}
	}


	static void PopulateNeighbours (int _x, int _y, int _z) {
		for (int n = 0; n < currentLevel[_z, _x, _y].neighbours.Length; n++) {
			int newX = 0;
			int newY = 0;
			int newZ = 0;
			
			switch ((Direction)n) {
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
			
			if (newX < 0 || newY < 0 || newX >= currentLevel.GetLength(1) || newY >= currentLevel.GetLength(2)) {
				currentLevel[_z, _x, _y].neighbours[n] = null;
				return;
			}
			if (Levels.terrains[PlayerData.currentGame][_z, newX, newY] == 0) {
				currentLevel[_z, _x, _y].neighbours[n] = currentLevel[_z, newX, newY];
			}
		}
	}


	static void PopulateCellModels (int _x, int _y, int _z) {
		int terrain = Levels.terrains[PlayerData.currentGame][_z, _x, _y];
		if (terrain == -1) {
			return;
		}

		GameObject cellModel = GameObject.Instantiate(GameController.terrainModels[terrain]);
		Transform cellModelTransform = cellModel.GetComponent<Transform> ();
		cellModelTransform.SetParent (map.GetComponent<Transform>());
		cellModelTransform.localPosition = GetWorldPosition (_x, _y, _z);
		currentLevel[_z, _x, _y].terrainModel = cellModel;
	}


	static void PopulateUnits () {
		foreach (var _unit in currentUnits) {
			// HACK
			_unit.cell = currentLevel[_unit.cell.z, _unit.cell.x, _unit.cell.y];

			_unit.unitContainer = Units.GetUnitContainer(_unit);
			Transform unitTransform = _unit.unitContainer.GetComponent<Transform> ();
			unitTransform.position = _unit.cell.terrainModel.GetComponent<Transform>().position;
			
			if (_unit.id == 0) {
				player = _unit;
			}
		}
	}
	

	static Vector3 GetWorldPosition (int _x, int _y, int _z) {
		float x, y, z;
		if(GetShift(_x, 1, 0) == 0) {
			x = _x * hexOffsetX;
			y = _y * hexOffsetY;
		} else {
			x = _x * hexOffsetX;
			y = (_y + 0.5f) * hexOffsetY;
		}

		return new Vector3 (x, _z*0.7f, y);
	}


	static int GetShift (int _x, int _shift, int _s) {
		if ((_x + _s) % 2 == 0) {
			return _shift;
		} else {
			return 0;
		}
	}


	static void CurrentLevelLoop (LoopFunction loopFunction) {
		for (int z = 0; z < currentLevel.GetLength(0); z++ ) {
			for (int x = 0; x < currentLevel.GetLength(1); x++ ) {
				for (int y = 0; y < currentLevel.GetLength(2); y++ ) {
					loopFunction(x, y, z);
				}
			}
		}
	}
}
