using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GenericData;
using UnityEngine.UI;

public static class Map {
	public static Cell[,] currentLevel;
	static GameObject map;
	static Unit player;

	// World Settings
	static float hexSpeed = 6;
	static Vector3 hexSmallScale = new Vector3 (0.05f, 0.05f, 0.05f);
	static float hexOffsetX, hexOffsetY;
//	static int mapW, mapH;

	public static void Init () {
		Debug.Log ("Map.Init()");

		// Init UI
		GameController.uiMap.SetActive(true);
		Button menuButton = GameController.uiMap.GetComponent<Transform>().FindChild("Menu").GetComponent<Button>();
		menuButton.onClick.AddListener(() => {
			UI.Init();
		});

		// Init map
		map = GameObject.Instantiate (GameController.mapContainer);
		
		// Calculate hex offsets
		float d = 0.5f / (Mathf.Sqrt(3)/2);
		hexOffsetX = d * 1.5f;
		hexOffsetY = d * Mathf.Sqrt(3);

		// Fill cells for current level
		if (currentLevel == null) {
			currentLevel = GetLevel(PlayerData.playerLevel);
			SaveLoad.Save (currentLevel, SaveLoad.levelDataFileName);
		}

		if (player == null) {
			foreach (var _cell in currentLevel) {
				if (_cell.unit != null && _cell.unit.id == 0) {
					player = _cell.unit;
					break;
				}
			}
		}

		// Draw map
		UpdateMap ();
	}


	static Cell[,] GetLevel (int _playerLevel) {
		int levelW = Levels.unitsAndItems[_playerLevel].GetLength(0);
		int levelH = Levels.unitsAndItems[_playerLevel].GetLength(1);
		Cell[,] newCells = new Cell[levelW,levelH];

		for (int x = 0; x < Levels.unitsAndItems[_playerLevel].GetLength(0); x++ ){
			for (int y = 0; y < Levels.unitsAndItems[_playerLevel].GetLength(1); y++ ){
				newCells[x, y] = new Cell();
				newCells[x, y].unitsAndItems = Levels.unitsAndItems[_playerLevel][x, y];
			}
		}
		return newCells;
	}


	public static void UpdateMap () {
		Debug.Log ("UpdateMap()");
//		Units.units = new List<GameObject>();
//
//		if (_shiftX != 0) {
//			if (Player.x % 2 != 0) {
//				_shiftY = 1;
//			}
//			Overview.shift = !Overview.shift;
//		}
//
//		Player.x += _shiftX;
//		Player.y += _shiftY;
//
//		// Create an temporary array and list
//		mapW = Overview.GetMask(Player.overview).GetLength(0);
//		mapH = Overview.GetMask(Player.overview).GetLength(1);
//
//
//		// Find player's cell
//		playerCellX = Mathf.RoundToInt(mapW/2);
//		playerCellY = mapH - 2;
//
//		// Declarate new map array
//		Cell[,] newCells = new Cell[mapW, mapH];
//		List<Cell> copyed = new List<Cell>();
//
//		// Add and copy cells to new array
//		for (int x = 0; x < mapW; x++) {
//			for (int y = 0; y < mapH; y++) {
//				if (Overview.GetMask(Player.overview)[x, y] != 0) {
//					if (cells != null) {
//						if (x + _shiftX >= 0 && x + _shiftX < mapW) {
//							if (y - _shiftY < mapW && y - _shiftY >= 0) {
//
//								Cell oldCell = cells[x + _shiftX, y - _shiftY];
//								if (oldCell != null) {
//									newCells[x, y] = oldCell;
//									newCells[x, y].arrayX = x;
//									newCells[x, y].arrayY = y;
//
//									copyed.Add (oldCell);
//									if (oldCell.GetComponent<Transform>().childCount > 1) {
//										GameObject unitContainer = oldCell.GetComponent<Transform>().GetChild(1).gameObject;
//										Units.units.Add (unitContainer);
//									}
//								}
//							}
//						}
//					}
//
//					if (cells == null || newCells[x,y] == null) {
//						newCells[x,y] = GetNewCell (x,y);
//					}
//					if (newCells[x, y] != null) {
//						newCells[x, y].GetComponent<Transform>().localPosition = GetWorldPosition(newCells[x, y].x, newCells[x, y].y);
//					}
//				}
//				if (cells != null) {
//					if (cells[x, y] != null) {
//						cells[x, y].GetComponent<Transform>().localPosition = GetWorldPosition(cells[x, y].x, cells[x, y].y);
//					}
//				}
//			}
//		}
//
//		// Remove tiles
//		if (cells != null) {
//			for (int x = 0; x < mapW; x++) {
//				for (int y = 0; y < mapH; y++) {
//					if (cells[x, y] != null && !copyed.Contains(cells[x, y])) {
//						cells[x, y].die = true;
//						cells[x, y].GetComponent<Size>().enabled = true;
//					}
//				}
//			}
//		}
//
//		if (cells == null) {
//			map.GetComponent<Transform>().position = GetPlayerWorldCoordinates();
//		}
//
//		cells = newCells;
//		map.GetComponent<Move>().enabled = true;
	}


//	public static Vector3 GetPlayerWorldCoordinates () {
//		Vector3 playerCell = GetWorldPosition(Player.x, Player.y);
//		return -playerCell;
//	}


//	public static GameObject GetCell (int _x, int _y) {
//		if (_x < 0 || _y < 0) {
//			return null;
//		}
//
//		if (_x > mapW -1 || _y > mapH -1) {
//			return null;
//		}
//
//		return cells[_x, _y];
//	}
//

//	static Cell GetNewCell (int _x, int _y) {
//		// Cell container
//		GameObject _cellContainer = GameObject.Instantiate (cellContainer);
//		Transform cellTransform = _cellContainer.GetComponent<Transform> ();
//
//		// Class
//		Cell cell = _cellContainer.GetComponent<Cell> ();
//
//		// Position
//		cell.arrayX = _x;
//		cell.arrayY = _y;
//
//		cell.x = _x - playerCellX + Player.x;
//		cell.y = playerCellY - _y + Player.y;
//
//		// Terrain model
//		cell.terrain = Terrain.GetTerrain (cell);
//		GameObject model = (GameObject) GameObject.Instantiate (Map.terrainModels [0]);
//		Transform modelTransform = model.GetComponent<Transform> ();
//		modelTransform.SetParent (cell.GetComponent<Transform> ());
//		modelTransform.localPosition = new Vector3 (0f, 0f, 0f);
//		modelTransform.localScale = new Vector3 (1f, 1f, 1f);
//		if (cell.terrain == 1) {
//			model.GetComponent<Renderer> ().material.color = new Color (0.6f, 0.65f, 0.6f);
//		}
//
//		cell.model = model;
//
//		// Unit
//		cell.unit = Terrain.GetUnit(cell);
//
//		if (cell.unit != -1) {
//			GameObject unitContainer = Units.GetUnit(cell.unit);
//			if (cell.unit == 0) {
//				Player.character = unitContainer;
//			}
//			if (cell.unit == 1) {
//				unitContainer.GetComponent<Transform> ().SetParent (cellTransform);
//			}
//		}
//
//		// Parent
//		cellTransform.SetParent (map.GetComponent<Transform> ());
//		cellTransform.localScale = hexSmallScale;
//
//		return cell;
//	}


	public static Vector3 GetWorldPosition (int _x, int _y) {
		float x, y;
		if( _x % 2 == 0 ) {
			x = _x * hexOffsetX;
			y = _y * hexOffsetY;
		} else {
			x = _x * hexOffsetX;
			y = (_y + 0.5f) * hexOffsetY;
		}

		return new Vector3 (x, 0f, y);
	}
}
