using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
	//Private data
	// Allowed paths to:
	// 1. avoid adges, cutt edges
	// 2. 256 available 3x3 chunk cases
	// 3. Allowed or prohibed way for one of 8 directions
	int[,] AllowedDirections = new int[256, 8];
	int pathfindingMax;

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

		pathfindingMax = (_level[0] * _level[1]) / 2;

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
				cellClass.DirectionLayers = new int[] {255,255};

				cellClass.terrain = GetTerrain ();
			}
		}
	}

	// TODO Map: Procedural map generation
	// TODO Map: hills
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

	Cell RockFilter (Cell _cell)
	{
		if (_cell.terrain == 1) {
			return null;
		} else {
			return _cell;
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
	
	// Tell to the neighbours that cell is closed now
	// And set to the neighbours movement index
	public void UpdateCellMask (Cell _cell, int _layer, int _set)
	{
		
		// Set zero index of available paths
		_cell.DirectionLayers [_layer] = _set;
		
		// Update ourself first if necessery
		if (_cell.DirectionLayers [_layer] != -1) {
			_cell.DirectionLayers [_layer] = CalculateCellMask (_cell, _layer);
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

	// Calculate new path
	public List<Cell> FindPath (Cell _source, Cell _target)
	{
		if (_source == _target) {
			Debug.LogWarning ("Pathfinding: source == target");
			return null;
		}

		Heap<Cell> opened = new Heap<Cell> (Cells.GetLength(0) * Cells.GetLength(1));
		HashSet<Cell> closed = new HashSet<Cell> ();

		opened.Add (_source);

		while (opened.Count > 0) {
			// Assign some active node as current
			Cell currentCell = opened.RemoveFirst();


			closed.Add (currentCell);

			if (currentCell == _target) {
				break;
			}

			// TODO Pathf: Make ReturnNeighbour(layer) function to the node
			// TODO Pathf: Detect unreacheble cell: terrain, building or unit? Stop, wait, collaborate or attack?

			// If our cell is start cell, probably it allready busie
			// We should calculate it neighbour map
			int map;
			if (currentCell.DirectionLayers [0] == -1) {
				map = CalculateCellMask (_source, 0);
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

				// Check for dynamic obstackle
				if (currentCell.neighbours [i].DirectionLayers [0] == -1)
					continue;

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

		if (!closed.Contains (_target)) {
			Debug.LogWarning ("Unreacheble goal! " + _source.x + " " + _source.y + ", " + _target.x + " " + _target.y + ". " + Time.timeSinceLevelLoad);
			Cell closestCell = null;
			int distance = 0;

			foreach (var cell in closed) {
				int altDistance = GetDistance(cell, _target);
				if (distance == 0 || altDistance < distance){
					closestCell = cell;
					distance = altDistance;
				}
			}

			// TODO Don't change the _target, but check to better path next time
			_target = closestCell;
		}

		if (closed.Count > 200)
			Debug.LogWarningFormat ("Count: " + closed.Count + ", Source: " + _source.x + " " + _source.y + ", Target: " + _target.x + " " + _target.y);
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

	public int GetDistance (Cell _source, Cell _target)
	{
		int distanceX = Mathf.Abs (_source.x - _target.x);
		int distanceY = Mathf.Abs (_source.y - _target.y);

		if (distanceX > distanceY) {
			return (14 * distanceY) + (10 * (distanceX - distanceY));
		}
		return (14 * distanceX) + (10 * (distanceY - distanceX));
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

	public void PlaceUnit (int _type)
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
