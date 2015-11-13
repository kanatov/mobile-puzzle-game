using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{

	// Allowed paths to:
	// 1. avoid adges, cutt edges
	// 2. 256 available 3x3 chunk cases
	// 3. Allowed or prohibed way for one of 8 directions
	int[,] AllowedDirections = new int[256, 8];

	// Public data
	public Main Main;
	public GameObject[,] Cells;
	public GameObject[] TerrainModels;
	public GameObject CellRoot;
	public GameObject UnitRoot;

	// Init new level
	public void Init (int[] _level)
	{
		Cells = new GameObject[_level [0], _level [1]];

		PopulateCell (Cells);
		PopulateNeighbours (Cells);
		PopulateAllowedDirections (AllowedDirections);
		PopulateGroundMap (Cells);
		DrawTerrain (Cells);

		for (int i = 0; i < _level[2]; i++) {
			PlaceUnit (0);
		}

		for (int i = 0; i < _level[3]; i++) {
			PlaceUnit (1);
		}

	}

	// Create cell GameObjects and add it to array
	void PopulateCell (GameObject[,] _map)
	{
		for (int x = 0; x < _map.GetLength(0); x++) {
			for (int y = 0; y < _map.GetLength(1); y++) {
				GameObject cell = (GameObject)GameObject.Instantiate (
					CellRoot,
					new Vector3 (x, 0f, y),
					Quaternion.Euler (new Vector3 (90, 0, 0))
				);
				cell.GetComponent<Transform> ().SetParent (this.GetComponent<Transform> ());

				_map [x, y] = cell;
				Cell cellClass = cell.GetComponent<Cell> ();
				cellClass.Main = Main;
				cellClass.x = x;
				cellClass.y = y;
				cellClass.DirectionLayers = new int[] {255,255,255,255};

				cellClass.terrain = GetTerrain ();
			}
		}
	}

	// TODO Map: Procedural map generation
	// Map terrain generator
	int GetTerrain ()
	{
		float range = UnityEngine.Random.Range (0.0f, 1.0f);
		int type = 0;
		
		if (range > 0.85f) {
			type = 1;
		}
		return type;
	}

	// Calculate all neigbours and add them to each other
	void PopulateNeighbours (GameObject[,] _map)
	{

		int width = _map.GetLength (0);
		int height = _map.GetLength (1);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Cell cellClass = _map [x, y].GetComponent<Cell> ();
				// Skip rocks
				if (cellClass.terrain != 1) {

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
						cellClass.neighbours [5] = RockFilter (_map [x, y - 1].GetComponent<Cell> ());
					
					//Bottom
					if (y < height - 1)
						cellClass.neighbours [1] = RockFilter (_map [x, y + 1].GetComponent<Cell> ());

					//Left
					if (x > 0) {
						cellClass.neighbours [7] = RockFilter (_map [x - 1, y].GetComponent<Cell> ());

						//Left Top
						if (y > 0)
							cellClass.neighbours [6] = RockFilter (_map [x - 1, y - 1].GetComponent<Cell> ());

						//Left Bottom
						if (y < height - 1)
							cellClass.neighbours [0] = RockFilter (_map [x - 1, y + 1].GetComponent<Cell> ());
					}
					
					// Right
					if (x < width - 1) {
						cellClass.neighbours [3] = RockFilter (_map [x + 1, y].GetComponent<Cell> ());

						//Right Top
						if (y > 0)
							cellClass.neighbours [4] = RockFilter (_map [x + 1, y - 1].GetComponent<Cell> ());

						//Right Bottom
						if (y < height - 1)
							cellClass.neighbours [2] = RockFilter (_map [x + 1, y + 1].GetComponent<Cell> ());
					}
				}
			}
		}
	}

	Cell RockFilter (Cell _cell)
	{
		if (_cell.terrain == 1) {
			return null;
		} else {
			return _cell;
		}
	}

	// Create movement map with terrain
	void PopulateGroundMap (GameObject[,] _map)
	{
		for (int x = 0; x < _map.GetLength(0); x++) {
			for (int y = 0; y < _map.GetLength(1); y++) {
				Cell cell = _map [x, y].GetComponent<Cell> ();
				if (cell.terrain == 1)
					UpdateCellMask (cell, 0, -1);
			}
		}
	}

	// Tell neighbours that cell is closed now
	// And set to the neighbours movement index
	public void UpdateCellMask (Cell _cell, int _layer, int _set)
	{

		// Set zero index of available paths
		_cell.DirectionLayers [_layer] = _set;

		// Update ourself first if necessery
		if (_cell.DirectionLayers [_layer] != -1) {
			CalculateCellMask (_cell, _layer);
		}

		// For all neighbour we know
		for (int i = 0; i < _cell.neighbours.Length; i++) {

			// If our neighbour is exist and not a zero
			if (_cell.neighbours [i] != null && _cell.neighbours [i].DirectionLayers [_layer] != -1) {
				_cell.neighbours [i].DirectionLayers [_layer] = CalculateCellMask (_cell.neighbours [i], _layer);
			}
		}
	}

	// Work with tile and it neighbours
	int CalculateCellMask (Cell _cell, int _layer)
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
	void DrawTerrain (GameObject[,] map)
	{
		for (int x = 0; x < map.GetLength (0); x++) {
			for (int y = 0; y < map.GetLength (1); y++) {
				Cell cell = map [x, y].GetComponent<Cell> ();
				GameObject terrain;

				terrain = (GameObject)GameObject.Instantiate (TerrainModels [cell.terrain]);
				terrain.GetComponent<Transform> ().SetParent (map [x, y].GetComponent<Transform> ());
				terrain.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0f, 0f);

				cell.normalColor = terrain.GetComponent<Renderer> ().material.color;
				cell.Tile = terrain;
			}
		}
	}

	// Create an index of movements: cut edges or not
	void PopulateAllowedDirections (int[,] _movementIndex)
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
				// If ID is false and _movementIndex == 1
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
	
	void Reverse (BitArray array)
	{
		int length = array.Length;
		int mid = (length / 2);
		
		for (int i = 0; i < mid; i++) {
			bool bit = array [i];
			array [i] = array [length - i - 1];
			array [length - i - 1] = bit;
		}    
	}
	
	// Calculate new path
	public List<Cell> FindPath (Cell _source, Cell _target)
	{
		if (_source == _target) {
			Debug.LogWarning ("Pathfinding: source == target");
			return null;
		}
		
		Dictionary<Cell, float> dist = new Dictionary<Cell, float> ();
		Dictionary<Cell, Cell> prev = new Dictionary<Cell, Cell> ();
		
		// Setup the "unvisited" -- the list of cells we haven't checked yet.
		List<Cell> unvisited = new List<Cell> ();
		
		dist [_source] = 0;
		prev [_source] = null;
		
		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some cells CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach (GameObject tile in Cells) {
			Cell v = tile.GetComponent<Cell> ();
			if (v != _source) {
				dist [v] = Mathf.Infinity;
				prev [v] = null;
			}
			unvisited.Add (v);
		}
		
		while (unvisited.Count > 0) {
			// "u" is going to be the unvisited cell with the smallest distance.
			Cell u = null;
			
			foreach (Cell possibleU in unvisited) {
				if (u == null || dist [possibleU] < dist [u]) {
					u = possibleU;
				}
			}
			
			if (u == _target) {
				break;
			}
			
			unvisited.Remove (u);

			// Detect target direction
			// If Tile in target's direction is acceptable:
			// Visiti it
			// else visit evry tile

			// For every neighbour of Unvisited cell
			for (int i = 0; i < u.neighbours.Length; i++) {
				if (u.neighbours [i] != null) {

					// V - it's current neighbour
					Cell v = u.neighbours [i];

					// Alternate coast is previous coast + of Unvisited cell + path to the neighbour
					float alt = dist [u] + TileCoast (u, v, i);

					if (alt < dist [v]) {
						// Alternate coast is less then previous neighbour coast
						// Apply alternate coast to the neighbour
						dist [v] = alt;
						prev [v] = u;
					}
				}
			}
		}
		
		
		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.
		if (prev [_target] == null) {
			
			// No route between our target and the source
			// Try to find closest cell
			Cell closestCell = null;
			
			int sourceLengthX = Mathf.Abs (_source.x - _target.x);
			int sourceLengthY = Mathf.Abs (_source.y - _target.y);
			int sourceLength = sourceLengthX + sourceLengthY;
			
			int shortestLength = sourceLength;
			
			// Try to find closest cell from available
			foreach (var c in prev) {
				if (c.Value != null) {
					int lengthX = Mathf.Abs (c.Key.x - _target.x);
					int lengthY = Mathf.Abs (c.Key.y - _target.y);
					int lengthSumm = lengthX + lengthY;
					
					if (shortestLength > lengthSumm) {
						closestCell = c.Key;
						shortestLength = lengthSumm;
					}
				}
			}
			
			if (shortestLength < sourceLength) {
				_target = closestCell;
			} else {
				return null;
			}
			
		}
		
		List<Cell> currentPath = new List<Cell> ();
		Cell curr = _target;
		
		// Step through the "prev" chain and add it to our path
		while (curr != null) {
			currentPath.Add (curr);
			curr = prev [curr];
		}
		
		currentPath.Reverse ();
		return currentPath;
	}

	// Tile Coast
	float TileCoast (Cell _source, Cell _target, int _direction)
	{
		//Base coast
		float coast = 1;

		if (_target.DirectionLayers [0] == -1) {
			coast = Mathf.Infinity;
		}

		int map = 0;
		if (_source.DirectionLayers [0] == -1) {
			map = CalculateCellMask (_source, 0);
		} else {
			map = _source.DirectionLayers [0];
		}

		if (AllowedDirections [map, _direction] == 0) {
			coast = Mathf.Infinity;
		}

		if (_source.x != _target.x && _source.y != _target.y) {
			coast += 0.001f;
		}
		
		return coast;
		
	}

	public Cell GetRandomPlace ()
	{
		Cell cell;

		do {
			cell = null;
			int x;
			int y;

			x = UnityEngine.Random.Range (0, Cells.GetLength (0));
			y = UnityEngine.Random.Range (0, Cells.GetLength (1));

			if (Cells [x, y].GetComponent<Cell> ().DirectionLayers [0] != -1) {
				cell = Cells [x, y].GetComponent<Cell> ();
			}
		} while (cell == null);

		return cell;
	}

	void PlaceUnit (int _type)
	{
		GameObject unit = (GameObject)GameObject.Instantiate (UnitRoot);

		Unit unitClass = unit.GetComponent<Unit> ();
		unitClass.Main = Main;
		unitClass.Map = this.GetComponent<Map> ();
		unitClass.Init (_type, GetRandomPlace ());
	}

	// Tile > World coordinates converter
	public Vector3 GetWorldCoordinates (Cell _cell)
	{
		return new Vector3 (_cell.x, 0f, _cell.y);
	}
}
