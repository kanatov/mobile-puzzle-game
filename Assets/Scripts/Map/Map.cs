using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class Map {
	// Public data
	public static GameObject map;
	public static Cell[,] cells;
	public static GameObject cellContainer;
	public static GameObject unitContainer;
	public static GameObject[] terrainModels;
	public static GameObject cameraContainer;

	public static float hexSpeed = 4;
	public static Vector3 hexSmallScale = new Vector3 (0.05f, 0.05f, 0.05f);

	static float hexOffsetX, hexOffsetY;
	static int mapW, mapH;
	static int playerCellX, playerCellY;


	public static void Init () {
		// Create map container
		if (map != null) {
			GameObject.Destroy(map.gameObject);
			cells = null;
		}

		cameraContainer.GetComponent<Camera>().SetPosition();
		map = GameObject.Instantiate (unitContainer);
		map.GetComponent<Transform>().position = new Vector3 (Player.x, 0f, Player.y);

		// Calculate hex offsets
		float d = 0.5f / (Mathf.Sqrt(3)/2);
		hexOffsetX = d * 1.5f;
		hexOffsetY = d * Mathf.Sqrt(3);

		//Init map
		Overview.Init();
		UpdateMap (0, 0);
	}


	public static void UpdateMap (int _shiftX, int _shiftY) {
		if (GameController.turnLock) {
			return;
		}

		if (_shiftX != 0) {
			if (Overview.GetShift) {
				_shiftY = -1;
			}
			Overview.shift = !Overview.shift;
		}

		Player.x += _shiftX;
		Player.y += _shiftY;

		// Create an temporary array and list
		mapW = Overview.GetMask(Player.overview).GetLength(0);
		mapH = Overview.GetMask(Player.overview).GetLength(1);

		// Find player's cell
		playerCellX = Mathf.RoundToInt(mapW/2);
		playerCellY = mapH - 2;

		// Declarate new map array
		Cell[,] newCells = new Cell[mapW, mapH];
		List<Cell> copyed = new List<Cell>();

		// Add and copy cells to new array
		for (int x = 0; x < mapW; x++) {
			for (int y = 0; y < mapH; y++) {
				if (Overview.GetMask(Player.overview)[x, y] != 0) {
					if (cells != null) {
						if (x + _shiftX >= 0 && x + _shiftX < mapW) {
							if (y + _shiftY >= 0) {
								if (cells[x + _shiftX, y + _shiftY] != null) {
									newCells[x, y] = cells[x + _shiftX, y + _shiftY];
									copyed.Add (newCells[x, y]);
								}
							}
						}
					}

					if (cells == null || newCells[x,y] == null) {
						newCells[x,y] = GetNewCell (x,y);
					}
					SetCellWorldPosition(newCells[x, y]);
				}
				if (cells != null) {
					SetCellWorldPosition(cells[x, y]);
				}
			}
		}

		// Remove tiles
		if (cells != null) {
			for (int x = 0; x < mapW; x++) {
				for (int y = 0; y < mapH; y++) {
					if (cells[x, y] != null && !copyed.Contains(cells[x, y])) {
						cells[x, y].GetComponent<Remove>().enabled = true;
					}
				}
			}
		}

		Player.source = newCells[playerCellX, playerCellY];
		if (cells == null) {
			map.GetComponent<Transform>().position = GetMapContainerPosition();
		}
		cells = newCells;
		map.GetComponent<Move>().enabled = true;
	}


	public static Vector3 GetMapContainerPosition () {
		Vector3 playerCell = Player.source.GetComponent<Transform>().localPosition;
		return playerCell * -1;
	}

	static Cell GetNewCell (int _x, int _y) {
		GameObject cellContainerObject = GameObject.Instantiate (cellContainer);
		cellContainerObject.GetComponent<Transform> ().SetParent (map.GetComponent<Transform> ());
		
		Cell cell = cellContainerObject.GetComponent<Cell> ();
		SetCellMapPosition (cell, _x, _y);
		Terrain.GetTerrain (cell);
		Units.GetUnit (cell);

		return cell;
	}


	static void SetCellWorldPosition (Cell _cell) {
		if (_cell == null) {
			return;
		}
		_cell.GetComponent<Transform> ().localPosition = GetWorldCoordinates(_cell.x, _cell.y);
	}


	static void SetCellMapPosition (Cell _cell, int _x, int _y) {
		if (_cell == null) {
			return;
		}
		_cell.x = _x + Player.x;
		_cell.y = _y + Player.y;
	}


	// Tile > World coordinates converter
	public static Vector3 GetWorldCoordinates (int _x, int _y) {
		float x, y;

		if( _x % 2 == 0 ) {
			x = _x * hexOffsetX;
			y = (_y + 0.5f) * hexOffsetY;
		} else {
			x = _x * hexOffsetX;
			y = _y * hexOffsetY;
		}
		return new Vector3 (x, 0f, -y);
	}
}
