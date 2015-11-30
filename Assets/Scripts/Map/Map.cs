using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class Map {
	// Public data
	public static GameObject map;
	public static Cell[,] cells;
	public static GameObject CellContainer;
	public static GameObject UnitContainer;
	public static GameObject[] TerrainModels;

	// Create Map
	public static void Init () {

		// Create map container
		map = GameObject.Instantiate (UnitContainer);
		map.GetComponent<Transform>().position = new Vector3 (Player.x, 0f, Player.y);

		Overview.Init();
		UpdateMap (0,0);
	}

	// set new player coordinates
	// if world coordinates != player coordinates
	// move world parent

	public static void UpdateMap (int _shiftX, int _shiftY) {

		if (_shiftX != 0) {
			Overview.shift = !Overview.shift;

			if (!Overview.shift) {
				_shiftY *= 0;
			}
		}

		Player.x += _shiftX;
		Player.y += _shiftY;


		// Copy and add tiles
		int cellSizeX = Overview.Get(Player.overview).GetLength(0);
		int cellSizeY = Overview.Get(Player.overview).GetLength(1);
		Cell[,] newCells = new Cell[cellSizeX, cellSizeY];
		List<Cell> copyed = new List<Cell>();

		for (int x = 0; x < newCells.GetLength(0); x++) {
			for (int y = 0; y < newCells.GetLength(1); y++) {
				if (Overview.Get(Player.overview)[x, y] != 0) {
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
					newCells[x, y].GetComponent<Transform> ().localPosition = GetWorldCoordinates(newCells[x, y]);
				}

				if (cells != null && cells[x, y] != null) {
					cells[x, y].GetComponent<Transform> ().localPosition = GetWorldCoordinates(cells[x, y]);
				}
			}
		}

		// Remove tiles and set coordinates
		if (cells != null) {
			for (int x = 0; x < newCells.GetLength(0); x++) {
				for (int y = 0; y < newCells.GetLength(1); y++) {
					if (cells[x, y] != null && !copyed.Contains(cells[x, y])) {
						cells[x, y].GetComponent<Remove>().enabled = true;
					}
				}
			}
		}
		cells = newCells;
		map.GetComponent<Move>().enabled = true;
	}

	// Tile > World coordinates converter
	public static Vector3 GetWorldCoordinates (Cell _cell) {
		float x, y;
			if( _cell.x % 2 == 0 ) {
				x = _cell.x * Terrain.offsetX;
				y = (_cell.y + 0.5f) * Terrain.offsetY;
			} else {
				x = _cell.x * Terrain.offsetX;
				y = _cell.y * Terrain.offsetY;
			}

		
		return new Vector3 (x, 0f, -y);
	}
	// Calculate all neigbours and add them to each other
//	static void PopulateNeighbours (Cell[,] _map)
//	{
//
//		int width = _map.GetLength (0);
//		int height = _map.GetLength (1);
//
//		for (int x = 0; x < width; x++) {
//			for (int y = 0; y < height; y++) {
//
//				// 0           1           2
//				//           
//				//   | -1, 1  0, 1  1, 1
//				//   |
//				// 7 | -1, 0  0, 0  1, 0   3
//				//   | 
//				//   | -1,-1  0,-1  1,-1 
//				//  y|________________
//				//  0 x
//				// 6           5           4
//
//				// Top
//				if (y > 0)
//					_map [x, y].neighbours [5] = RockFilter (_map [x, y - 1]);
//				
//				//Bottom
//				if (y < height - 1)
//					_map [x, y].neighbours [1] = RockFilter (_map [x, y + 1]);
//
//				//Left
//				if (x > 0) {
//					_map [x, y].neighbours [7] = RockFilter (_map [x - 1, y]);
//
//					//Left Top
//					if (y > 0)
//						_map [x, y].neighbours [6] = RockFilter (_map [x - 1, y - 1]);
//
//					//Left Bottom
//					if (y < height - 1)
//						_map [x, y].neighbours [0] = RockFilter (_map [x - 1, y + 1]);
//				}
//				
//				// Right
//				if (x < width - 1) {
//					_map [x, y].neighbours [3] = RockFilter (_map [x + 1, y]);
//
//					//Right Top
//					if (y > 0)
//						_map [x, y].neighbours [4] = RockFilter (_map [x + 1, y - 1]);
//
//					//Right Bottom
//					if (y < height - 1)
//						_map [x, y].neighbours [2] = RockFilter (_map [x + 1, y + 1]);
//				}
//			}
//		}
//	}

//	static Cell RockFilter (Cell _cell)
//	{
//		if (_cell.terrain == 1) {
//			return null;
//		} else {
//			return _cell;
//		}
//	}
//
//
//	// Create movement map with terrain
//	static void PopulateGroundMap (Cell[,] _map)
//	{
//		for (int x = 0; x < _map.GetLength(0); x++) {
//			for (int y = 0; y < _map.GetLength(1); y++) {
//				Cell cell = _map [x, y];
//				
//				if (cell.terrain == 1)
//					UpdateCellMask (cell, 0, false);
//			}
//		}
//	}
//	
//	public static int GetDistance (Cell _source, Cell _target)
//	{
//		int distanceX = Mathf.Abs (_source.x - _target.x);
//		int distanceY = Mathf.Abs (_source.y - _target.y);
//
//		if (distanceX > distanceY) {
//			return (14 * distanceY) + (10 * (distanceX - distanceY));
//		}
//		return (14 * distanceX) + (10 * (distanceY - distanceX));
//	}
//
//	public static Cell GetRandomPlace ()
//	{
//		Cell cell;
//
//		do {
//			cell = null;
//			int x;
//			int y;
//
//			x = UnityEngine.Random.Range (0, Cells.GetLength (0));
//			y = UnityEngine.Random.Range (0, Cells.GetLength (1));
//
//			if (Cells [x, y].DirectionLayers [0] != -1) {
//				cell = Cells [x, y];
//			}
//		} while (cell == null);
//
//		return cell;
//	}
//
}
