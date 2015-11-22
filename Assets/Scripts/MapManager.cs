using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class MapManager {
	//Private data
	static int[,] AllowedDirections = new int[256, 8];
	static int pathfindingMax;
	static GameObject map;

	// Debug
	static Color debugColor = Color.yellow;

	// Public data
	public static Cell[,] Cells;
	public static GameObject CellContainer;
	public static GameObject[] TerrainModels;

	// Create Map
	public static void Init (int[] _level)
	{
		// Looking for the old MapRoot instance and remove it
		GameObject[] mapInstances = GameObject.FindGameObjectsWithTag ("Map");
		foreach (var i in mapInstances) {
			GameObject.Destroy (i);
		}
		
		// Creating new map
		map = new GameObject();
		map.tag = "Map";

		Cells = new Cell[_level [0], _level [1]];
		
		pathfindingMax = (_level[0] * _level[1]) / 3;
		
		PopulateCell (Cells);
		PopulateNeighbours (Cells);
		PopulateAllowedDirections (AllowedDirections);
		PopulateGroundMap (Cells);
		DrawTerrain (Cells);
		
		for (int i = 0; i < _level[2]; i++) {
			UnitManager.Create (0);
		}
		
		for (int i = 0; i < _level[3]; i++) {
			UnitManager.Create (1);
		}
	}

	// Create cell GameObjects and add it to array
	static void PopulateCell (Cell[,] _map)
	{
		for (int x = 0; x < _map.GetLength(0); x++) {
			for (int y = 0; y < _map.GetLength(1); y++) {
				GameObject cell = (GameObject)GameObject.Instantiate (
					CellContainer,
					new Vector3 (x, 0f, y),
					Quaternion.Euler (new Vector3 (90, 0, 0))
				);
				cell.GetComponent<Transform> ().SetParent (map.GetComponent<Transform> ());

				_map [x, y] = cell.GetComponent<Cell> ();
				_map [x, y].x = x;
				_map [x, y].y = y;
				_map [x, y].DirectionLayers = new int[] {255,255};
				_map [x, y].terrain = GetTerrain ();
			}
		}
	}

	// TODO Map: Procedural map generation
	// TODO Map: hills
	// Map terrain generator
	static int GetTerrain ()
	{
		float range = UnityEngine.Random.Range (0.0f, 1.0f);
		int type = 0;
		
		if (range > 0.9f) {
			type = 1;
		}
		return type;
	}

	// Calculate all neigbours and add them to each other
	static void PopulateNeighbours (Cell[,] _map)
	{

		int width = _map.GetLength (0);
		int height = _map.GetLength (1);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				// 0           1           2
				//           
				//   | -1, 1  0, 1  1, 1
				//   |
				// 7 | -1, 0  0, 0  1, 0   3
				//   | 
				//   | -1,-1  0,-1  1,-1 
				//  y|________________
				//  0 x
				// 6           5           4

				// Top
				if (y > 0)
					_map [x, y].neighbours [5] = RockFilter (_map [x, y - 1]);
				
				//Bottom
				if (y < height - 1)
					_map [x, y].neighbours [1] = RockFilter (_map [x, y + 1]);

				//Left
				if (x > 0) {
					_map [x, y].neighbours [7] = RockFilter (_map [x - 1, y]);

					//Left Top
					if (y > 0)
						_map [x, y].neighbours [6] = RockFilter (_map [x - 1, y - 1]);

					//Left Bottom
					if (y < height - 1)
						_map [x, y].neighbours [0] = RockFilter (_map [x - 1, y + 1]);
				}
				
				// Right
				if (x < width - 1) {
					_map [x, y].neighbours [3] = RockFilter (_map [x + 1, y]);

					//Right Top
					if (y > 0)
						_map [x, y].neighbours [4] = RockFilter (_map [x + 1, y - 1]);

					//Right Bottom
					if (y < height - 1)
						_map [x, y].neighbours [2] = RockFilter (_map [x + 1, y + 1]);
				}
			}
		}
	}

	static Cell RockFilter (Cell _cell)
	{
		if (_cell.terrain == 1) {
			return null;
		} else {
			return _cell;
		}
	}

	// Create an index of movements: cut edges or not
	static void PopulateAllowedDirections (int[,] _movementIndex)
	{
		for (int i = 0; i < _movementIndex.GetLength(0); i++) {
			for (int n = 0; n < _movementIndex.GetLength(1); n++) {
				// Mowement in all directions are allowed by default
				_movementIndex [i, n] = 1;
			}
		}

		for (int i = 0; i < _movementIndex.GetLength(0); i++) {
			// Convert i to bits
			BitArray bits = new BitArray (new int[] {i});
			BitArray id = new BitArray (8);

			for (int j = 0; j < id.Length; j++) {
				id [j] = bits [j];
			}
			// Reversing bitArray
			Reverse (id);

			for (int n = 0; n < _movementIndex.GetLength(1); n++) {
				// Populate allowed paths depends on movement type
				// If id is false and _movementIndex == 1
				if (!id [n] && _movementIndex [i, n] == 1) {
					_movementIndex [i, n] = 0;

					if (n % 2 != 0) {
						_movementIndex [i, n - 1] = 0;

						if (n == 7) {
							_movementIndex [i, 0] = 0;
						} else {
							_movementIndex [i, n + 1] = 0;
						}
					}
				}
			}
		}
	}
	
	static void Reverse (BitArray array)
	{
		int length = array.Length;
		int mid = (length / 2);
		
		for (int i = 0; i < mid; i++) {
			bool bit = array [i];
			array [i] = array [length - i - 1];
			array [length - i - 1] = bit;
		}    
	}

	// Create movement map with terrain
	static void PopulateGroundMap (Cell[,] _map)
	{
		for (int x = 0; x < _map.GetLength(0); x++) {
			for (int y = 0; y < _map.GetLength(1); y++) {
				Cell cell = _map [x, y];
				
				if (cell.terrain == 1)
					UpdateCellMask (cell, 0, false);
			}
		}
	}
	
	// Tell to the neighbours that cell is closed now
	// And set to the neighbours movement index
	public static void UpdateCellMask (Cell _cell, int _layer, bool _walkable)
	{
		
		// Set zero index of available paths
		if (_walkable) {
			_cell.DirectionLayers [_layer] = CalculateCellMask (_cell, _layer);
		} else {
			_cell.DirectionLayers [_layer] = -1;
		}
		
		// For all neighbour we know
		for (int i = 0; i < _cell.neighbours.Length; i++) {
			
			if (_cell.neighbours [i] == null)
				continue;
			
			if (_cell.neighbours [i].DirectionLayers [_layer] == -1)
				continue;
			
			_cell.neighbours [i].DirectionLayers [_layer] = CalculateCellMask (_cell.neighbours [i], _layer);
		}
	}
	
	// Work with tile and it neighbours
	static int CalculateCellMask (Cell _cell, int _layer)
	{
		string index = "";
		
		// Looking for closed neighbours in current layer
		// And collect it to binary string
		foreach (var neighbour in _cell.neighbours) {
			if (neighbour != null && neighbour.DirectionLayers [_layer] != -1) {
				index += "1";
			} else {
				index += "0";
			}
		}
		
		// Apply index to cell
		return Convert.ToInt32 (index, 2);
	}
	
	// Draw map tiles on scene
	static void DrawTerrain (Cell[,] map)
	{
		for (int x = 0; x < map.GetLength (0); x++) {
			for (int y = 0; y < map.GetLength (1); y++) {
				Cell cell = map [x, y];
				GameObject terrain;
				
				terrain = (GameObject)GameObject.Instantiate (TerrainModels [cell.terrain]);
				terrain.GetComponent<Transform> ().SetParent (map [x, y].GetComponent<Transform> ());
				terrain.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0f, 0f);
				
				cell.normalColor = terrain.GetComponent<Renderer> ().material.color;
				cell.Tile = terrain;
				cell.material = cell.Tile.GetComponent<Renderer>().material;

			}
		}
	}

	// Calculate new path
	public static List<Cell> FindPath (Cell _source, Cell _target)
	{
		if (_source == null || _target == null) {
			Debug.LogWarning ("Pathfinding: source == " + _source + ", target == " + _target);
			return null;
		}

		if (_source == _target) {
			Debug.LogWarning ("Pathfinding: source == target");
			return null;
		}

		Heap<Cell> opened = new Heap<Cell> (Cells.GetLength(0) * Cells.GetLength(1));
		HashSet<Cell> closed = new HashSet<Cell> ();

		opened.Add (_source);

		// HACK
		bool recover = false;

		while (opened.Count > 0) {
			// Assign some active node as current
			Cell currentCell = opened.RemoveFirst();

			closed.Add (currentCell);


			if (currentCell == _target) {
				if (recover) {
					// HACK
					UpdateCellMask(_target, 0, false);
				}

				break;
			}

			// HACK
			int map;
			if (currentCell == _source) {
				map = CalculateCellMask (currentCell, 0);
			} else {
				map = currentCell.DirectionLayers [0];
			}


			// For every neighbour of current cell
			for (int i = 0; i < currentCell.neighbours.Length; i++) {
				// Check for an empty neighbour
				if (currentCell.neighbours [i] == null)
					continue;

				if (closed.Contains (currentCell.neighbours [i]))
					continue;

				if (currentCell.neighbours [i] == _target && currentCell.neighbours [i].DirectionLayers[0] == -1) {
					UpdateCellMask(currentCell.neighbours [i], 0, true);
					recover = true;
				}

				// Check for allowed direction
				if (AllowedDirections [map, i] == 0)
					continue;

				int newMovementCostToNeghbour = currentCell.gCost + GetDistance (currentCell, currentCell.neighbours [i]);

				if (newMovementCostToNeghbour < currentCell.neighbours [i].gCost || !opened.Contains (currentCell.neighbours [i])) {
					currentCell.neighbours [i].gCost = newMovementCostToNeghbour;
					currentCell.neighbours [i].hCost = GetDistance (currentCell.neighbours [i], _target);

					currentCell.neighbours [i].parent = currentCell;

					if (!opened.Contains (currentCell.neighbours [i]) && closed.Count < pathfindingMax)
						opened.Add (currentCell.neighbours [i]);
				}
			}
		}

		if (closed.Count >= pathfindingMax)
			Debug.LogWarningFormat ("Count: " + closed.Count + ", Source: " + _source.x + " " + _source.y + ", Target: " + _target.x + " " + _target.y);

		if (!closed.Contains (_target)) {
			Debug.LogWarning ("Unreacheble goal! " + _source.x + " " + _source.y + ", " + _target.x + " " + _target.y + ". " + Time.timeSinceLevelLoad);
			Cell closestCell = _source;
			int distance = GetDistance (_source, _target);

			foreach (var cell in closed) {
				int altDistance = GetDistance(cell, _target);
				if (altDistance < distance){
					closestCell = cell;
					distance = altDistance;
				}
			}

			if (closestCell == _source) {
				return null;
			} else {
				_target = closestCell;
			}
		}

		List<Cell> path = new List<Cell> ();
		Cell tmpCell = _target;

		while (tmpCell != _source) {
			path.Add (tmpCell);
			tmpCell = tmpCell.parent;
		}
		path.Add (_source);

		path.Reverse ();
		return path;
	}

	public static int GetDistance (Cell _source, Cell _target)
	{
		int distanceX = Mathf.Abs (_source.x - _target.x);
		int distanceY = Mathf.Abs (_source.y - _target.y);

		if (distanceX > distanceY) {
			return (14 * distanceY) + (10 * (distanceX - distanceY));
		}
		return (14 * distanceX) + (10 * (distanceY - distanceX));
	}

	public static Cell GetRandomPlace ()
	{
		Cell cell;

		do {
			cell = null;
			int x;
			int y;

			x = UnityEngine.Random.Range (0, Cells.GetLength (0));
			y = UnityEngine.Random.Range (0, Cells.GetLength (1));

			if (Cells [x, y].DirectionLayers [0] != -1) {
				cell = Cells [x, y];
			}
		} while (cell == null);

		return cell;
	}

	// Tile > World coordinates converter
	public static Vector3 GetWorldCoordinates (Cell _cell)
	{
		return new Vector3 (_cell.x, 0f, _cell.y);
	}


	//
	// Debug
	//


	public static void DebugDrawPath(Unit _unit){
		if (_unit != UnitManager.Players[0]) {
			return;
		}

		if (_unit.pathVis == null) {
			return;
		}

		int currCell = 0;
		
		while (currCell < _unit.pathVis.Count - 1) {
			Cell startCell = _unit.pathVis [currCell];
			Vector3 start = GetWorldCoordinates (startCell);
			
			Cell endCell = _unit.pathVis [currCell + 1];
			Vector3 end = GetWorldCoordinates (endCell);
			
			Debug.DrawLine (start, end, debugColor);
			
			currCell++;
		}
	}

	public static void DebugDrawClosedCells ()
	{
		for (int x = 0; x < Cells.GetLength (0); x++) {
			for (int y = 0; y < Cells.GetLength (1); y++) {
				if (Cells [x, y].terrain == 0) {
					if (Cells [x, y].DirectionLayers [1] != -1) {
						DebugNormalTile(Cells [x, y]);
					} else {
						DebugHighliteTile(Cells [x, y], 0.6f, 0.6f, 0.3f);
					}
				}
			}
		}
	}

	public static void DebugHighliteTile (Cell _cell, float _r, float _g, float _b)
	{
		GameObject tile = _cell.Tile;

		if (tile != null) {
			if (tile.GetComponent<Renderer> ().material.color != _cell.highliteColor) {
				tile.GetComponent<Renderer> ().material.color = new Color (_r, _g, _b);
			}
		}
	}
	
	public static void DebugNormalTile (Cell _cell)
	{
		GameObject tile = _cell.Tile;

		if (tile != null) {
			tile.GetComponent<Renderer> ().material.color = _cell.normalColor;
		}
	}
}
