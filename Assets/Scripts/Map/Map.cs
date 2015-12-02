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
			if (!Overview.shift) {
				_shiftY = 1;
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
							if (y - _shiftY < mapW && y - _shiftY >= 0) {
								Cell oldCell = cells[x + _shiftX, y - _shiftY];
								if (oldCell != null) {
									newCells[x, y] = oldCell;
									copyed.Add (oldCell);
								}
							}
						}
					}

					if (cells == null || newCells[x,y] == null) {
						newCells[x,y] = GetNewCell (x,y);
					}
					SetWorldPosition(newCells[x, y], x, y);
				}
				if (cells != null) {
					SetWorldPosition(cells[x, y], x - _shiftX, y - _shiftY);
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
		return -playerCell;
	}


	static Cell GetNewCell (int _x, int _y) {
		GameObject cellContainerObject = GameObject.Instantiate (cellContainer);
		cellContainerObject.GetComponent<Transform> ().SetParent (map.GetComponent<Transform> ());
		
		Cell cell = cellContainerObject.GetComponent<Cell> ();
		cell.x = _x - playerCellX + Player.x;
		cell.y = playerCellY - _y + Player.y;

		Terrain.GetTerrain (cell);
		Units.GetUnit (cell);

		return cell;
	}


	static void SetWorldPosition (Cell _cell, int _gridX, int _gridY) {
		if (_cell == null) {
			return;
		}
		float x, y;
		if( _cell.x % 2 == 0 ) {
			x = _cell.x * hexOffsetX;
			y = _cell.y * hexOffsetY;
		} else {
			x = _cell.x * hexOffsetX;
			y = (_cell.y + 0.5f) * hexOffsetY;
		}

		_cell.GetComponent<Transform> ().localPosition = new Vector3 (x, 0f, y);
	}
}
