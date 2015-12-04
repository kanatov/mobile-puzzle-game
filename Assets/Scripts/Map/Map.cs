using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class Map {
	// Public data
	public static GameObject map;
	public static Cell[,] cells;

	public static GameObject mapContainer;
	public static GameObject cellContainer;
	public static GameObject unitContainer;
	public static GameObject[] terrainModels;
	public static GameObject cameraContainer;

	public static float hexSpeed = 6;
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
		map = GameObject.Instantiate (mapContainer);


		// Calculate hex offsets
		float d = 0.5f / (Mathf.Sqrt(3)/2);
		hexOffsetX = d * 1.5f;
		hexOffsetY = d * Mathf.Sqrt(3);

		//Init map
		UpdateMap (0, 0);
	}


	public static void UpdateMap (int _shiftX, int _shiftY) {
		if (GameController.TurnLock != 0) {
			Debug.Log ("Turnlock: " + GameController.TurnLock);
			return;
		}

		GameController.enemyTurn = true;
		Units.enemies = new List<GameObject>();

		if (_shiftX != 0) {
			if (Player.x % 2 != 0) {
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
									newCells[x, y].arrayX = x;
									newCells[x, y].arrayY = y;

									copyed.Add (oldCell);
									if (oldCell.GetComponent<Transform>().childCount > 1) {
										GameObject enemy = oldCell.GetComponent<Transform>().GetChild(1).gameObject;
										Units.enemies.Add (enemy);
									}
								}
							}
						}
					}

					if (cells == null || newCells[x,y] == null) {
						newCells[x,y] = GetNewCell (x,y);
					}
					if (newCells[x, y] != null) {
						newCells[x, y].GetComponent<Transform>().localPosition = GetWorldPosition(newCells[x, y].x, newCells[x, y].y);
					}
				}
				if (cells != null) {
					if (cells[x, y] != null) {
						cells[x, y].GetComponent<Transform>().localPosition = GetWorldPosition(cells[x, y].x, cells[x, y].y);
					}
				}
			}
		}

		// Remove tiles
		if (cells != null) {
			for (int x = 0; x < mapW; x++) {
				for (int y = 0; y < mapH; y++) {
					if (cells[x, y] != null && !copyed.Contains(cells[x, y])) {
						cells[x, y].die = true;
						cells[x, y].GetComponent<Size>().enabled = true;
					}
				}
			}
		}

		if (cells == null) {
			map.GetComponent<Transform>().position = GetPlayerWorldCoordinates();
		}

		cells = newCells;
		map.GetComponent<Move>().enabled = true;
	}


	public static Vector3 GetPlayerWorldCoordinates () {
		Vector3 playerCell = GetWorldPosition(Player.x, Player.y);
		return -playerCell;
	}


	public static Cell GetCell (int _x, int _y) {
		if (_x < 0 || _y < 0) {
			return null;
		}

		if (_x > mapW -1 || _y > mapH -1) {
			return null;
		}

		return cells[_x, _y];
	}


	static Cell GetNewCell (int _x, int _y) {
		GameObject cellContainerObject = GameObject.Instantiate (cellContainer);
		cellContainerObject.GetComponent<Transform> ().SetParent (map.GetComponent<Transform> ());
		cellContainerObject.GetComponent<Transform> ().localScale = hexSmallScale;
		
		Cell cell = cellContainerObject.GetComponent<Cell> ();
		cell.arrayX = _x;
		cell.arrayY = _y;

		cell.x = _x - playerCellX + Player.x;
		cell.y = playerCellY - _y + Player.y;

		Terrain.GetTerrain (cell);
		Units.GetUnit (cell);

		return cell;
	}


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
