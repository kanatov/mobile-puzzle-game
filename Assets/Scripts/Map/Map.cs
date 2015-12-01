using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class Map {
	// Public data
	public static GameObject map;
	public static Cell[,] cells;
	public static GameObject CellContainer;
	public static GameObject unitContainer;
	public static GameObject[] terrainModels;

	public static float hexSpeed = 4;
	public static Vector3 hexSmallScale = new Vector3 (0.05f, 0.05f, 0.05f);

	static float hexOffsetX, hexOffsetY;


	public static void Init () {
		// Create map container
		if (map != null) {
			GameObject.Destroy(map.gameObject);
			cells = null;
		}

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

		// Copy and add tiles
		int cellSizeX = Overview.GetMask(Player.overview).GetLength(0);
		int cellSizeY = Overview.GetMask(Player.overview).GetLength(1);
		Cell[,] newCells = new Cell[cellSizeX, cellSizeY];
		List<Cell> copyed = new List<Cell>();

		for (int x = 0; x < newCells.GetLength(0); x++) {
			for (int y = 0; y < newCells.GetLength(1); y++) {
				if (Overview.GetMask(Player.overview)[x, y] != 0) {
					if (cells != null) {
						if (x + _shiftX >= 0 && y + _shiftY >= 0) {
							if (x + _shiftX < cellSizeX && y + _shiftY < cellSizeY) {
								if (cells[x + _shiftX, y + _shiftY] != null) {
									newCells[x, y] = cells[x + _shiftX, y + _shiftY];
									copyed.Add (newCells[x, y]);
								}
							}
						}
					}

					if (cells == null || newCells[x,y] == null) {
						GameObject cell = GameObject.Instantiate (CellContainer);
						cell.GetComponent<Transform> ().SetParent (map.GetComponent<Transform> ());
						
						newCells[x, y] = cell.GetComponent<Cell> ();
						newCells[x, y].x = x + Player.x;
						newCells[x, y].y = y + Player.y;
						Terrain.SetTerrain (newCells[x, y]);
					}
				}
				if (newCells[x, y] != null) {
					newCells[x, y].GetComponent<Transform> ().localPosition = GetWorldCoordinates(newCells[x, y].x, newCells[x, y].y);
				}

				if (cells != null && cells[x, y] != null) {
					cells[x, y].GetComponent<Transform> ().localPosition = GetWorldCoordinates(cells[x, y].x, cells[x, y].y);
				}
			}
		}

		int playerCellX = Mathf.RoundToInt(newCells.GetLength(0)/2);
		int playerCellY = newCells.GetLength(1) - 2;
		Player.source = newCells[playerCellX, playerCellY];

		// Remove tiles and set coordinates
		if (cells != null) {
			for (int x = 0; x < newCells.GetLength(0); x++) {
				for (int y = 0; y < newCells.GetLength(1); y++) {
					if (cells[x, y] != null && !copyed.Contains(cells[x, y])) {
						cells[x, y].GetComponent<Remove>().enabled = true;
					}
				}
			}
		} else {
			map.GetComponent<Transform>().position = GetZeroPosition();
		}

		cells = newCells;
		map.GetComponent<Move>().enabled = true;
	}

	public static Vector3 GetZeroPosition () {
		Vector3 ztLocal = Player.source.GetComponent<Transform>().localPosition;
		return ztLocal * -1;
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
