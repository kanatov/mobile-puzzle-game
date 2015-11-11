using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Map {

	//Private data
	Main Main;
	public GameObject[,] NodeMap;
	GameObject MapRootPrefab;
	GameObject MapRoot;
	GameObject NodePrefab;

	// Allowed paths to:
	// 1. avoid adges, cutt edges
	// 2. 256 available 3x3 chunk cases
	// 3. Allowed or prohibed way for one of 8 directions
	public int[,] AllowedDirections = new int[256,8];

	// Constructor - base map param
	public Map (Main _main, GameObject _MapRootPrefab, GameObject _NodePrefab, int _mapWidth, int _mapHeight) {
		Main = _main;
		MapRootPrefab = _MapRootPrefab;
		NodePrefab = _NodePrefab;

		// Create the new array for nodes
		NodeMap = new GameObject[_mapWidth, _mapHeight];

		// Everything is ready, let's create the map
		NewMap ();
	}

	void NewMap() {
		// Looking for the old MapRoot instance and remove it
		GameObject[] mapInstances = GameObject.FindGameObjectsWithTag ("MapRoot");
		foreach (var i in mapInstances) {
			GameObject.Destroy(i);
		}

		MapRoot = GameObject.Instantiate (MapRootPrefab);

		Debug.Log ("Start: PopulateNode");
		PopulateNode (NodeMap);
		Debug.Log ("Start: PopulateNeighbours");
		PopulateNeighbours (NodeMap);
		Debug.Log ("Start: PopulateAllowedDirections, 0");
		PopulateAllowedDirections (AllowedDirections);
		Debug.Log ("Start: PopulateGraph");
		PopulateGroundMap (NodeMap);
		Debug.Log ("Start: DrawMap");
		DrawTerrain (NodeMap);
	}

	// Create node GameObjects and add it to array
	void PopulateNode(GameObject[,] _map) {
		for (int x = 0; x < _map.GetLength(0); x++) {
			for (int y = 0; y < _map.GetLength(1); y++) {
				GameObject node = (GameObject) GameObject.Instantiate(
					NodePrefab,
					new Vector3(x, 0f, y),
					Quaternion.Euler(new Vector3(90, 0, 0))
					);
				node.GetComponent<Transform>().SetParent(MapRoot.GetComponent<Transform>());

				_map[x, y] = node;
				Node nodeClass = node.GetComponent<Node>();
				nodeClass.Main = Main;
				nodeClass.x = x;
				nodeClass.y = y;
				nodeClass.DirectionLayers = new int[] {255,255,255,255};

				nodeClass.terrain = GetTerrain();
			}
		}
	}

	// Map terrain generator
	int GetTerrain () {
		float range = UnityEngine.Random.Range (0.0f, 1.0f);
		int type = 0;
		
		if (range > 0.85f){
			type = 1;
		}
		return type;
	}

	// Calculate all neigbours and add them to each other
	void PopulateNeighbours(GameObject[,] _map) {

		int width = _map.GetLength (0);
		int height = _map.GetLength (1);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Node nodeClass = _map[x, y].GetComponent<Node>();
				// Skip rocks
				if (nodeClass.terrain != 1) {

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
						nodeClass.neighbours[5] = RockFilter(_map[x, y - 1].GetComponent<Node>());
					
					//Bottom
					if (y < height - 1)
						nodeClass.neighbours[1] = RockFilter(_map[x, y + 1].GetComponent<Node>());

					//Left
					if (x > 0) {
						nodeClass.neighbours[7] = RockFilter(_map[x - 1, y].GetComponent<Node>());

						//Left Top
						if (y > 0)
							nodeClass.neighbours[6] = RockFilter(_map[x-1, y - 1].GetComponent<Node>());

						//Left Bottom
						if (y < height - 1)
							nodeClass.neighbours[0] = RockFilter(_map[x-1, y + 1].GetComponent<Node>());
					}
					
					// Right
					if (x < width - 1) {
						nodeClass.neighbours[3] = RockFilter(_map[x + 1, y].GetComponent<Node>());

						//Right Top
						if (y > 0)
							nodeClass.neighbours[4] = RockFilter(_map[x+1, y - 1].GetComponent<Node>());

						//Right Bottom
						if (y < height - 1)
							nodeClass.neighbours[2] = RockFilter(_map[x+1, y + 1].GetComponent<Node>());
					}
				}
			}
		}
	}

	Node RockFilter(Node _node) {
		if (_node.terrain == 1) {
			return null;
		} else {
			return _node;
		}
	}

	// Create movement map with terrain
	void PopulateGroundMap(GameObject[,] _map) {
		for (int x = 0; x < _map.GetLength(0); x++) {
			for (int y = 0; y < _map.GetLength(1); y++) {
				Node node = _map[x, y].GetComponent<Node>();
				if (node.terrain == 1)
					UpdateNodeMask(node, 0, -1);
			}
		}
	}

	// Tell neighbours that node is closed now
	// And set to the neighbours movement index
	public void UpdateNodeMask (Node _node, int _layer, int _set) {

		// Set zero index of available paths
		_node.DirectionLayers[_layer] = _set;

		// Update ourself first if necessery
		if (_node.DirectionLayers [_layer] != -1) {
			CalculateNodeMask (_node, _layer);
		}

		// For all neighbour we know
		for (int i = 0; i < _node.neighbours.Length; i++) {

			// If our neighbour is exist and not a zero
			if (_node.neighbours[i] != null && _node.neighbours[i].DirectionLayers[_layer] != -1) {
				_node.neighbours[i].DirectionLayers[_layer] = CalculateNodeMask (_node.neighbours[i], _layer);
			}
		}
	}

	// Work with tile and it neighbours
	int CalculateNodeMask(Node _node, int _layer) {
		string index = "";
		
		// Looking for closed neighbours in current layer
		// And collect it to binary string
		foreach (var neighbour in _node.neighbours) {
			if (neighbour != null && neighbour.DirectionLayers[_layer] != -1) {
				index += "1";
			} else {
				index += "0";
			}
		}

		// Apply index to node
		return Convert.ToInt32(index, 2);
	}

	// Draw map tiles on scene
	void DrawTerrain(GameObject[,] map) {
		for (int x = 0; x < map.GetLength (0); x++) {
			for (int y = 0; y < map.GetLength (1); y++) {
				Node node = map[x, y].GetComponent<Node>();
				GameObject terrain;

				terrain = (GameObject) GameObject.Instantiate(Main.TerrainPrefab[node.terrain]);
				terrain.GetComponent<Transform>().SetParent(map[x, y].GetComponent<Transform>());
				terrain.GetComponent<Transform>().localPosition = new Vector3(0f,0f,0f);

				node.normalColor = terrain.GetComponent<Renderer> ().material.color;
				node.Tile = terrain;
			}
		}
	}

	// Create an index of movements: cut edges or not
	void PopulateAllowedDirections(int[,] _movementIndex) {
		for (int i = 0; i < _movementIndex.GetLength(0); i++) {
			for (int n = 0; n < _movementIndex.GetLength(1); n++) {
				// Mowement in all directions are allowed by default
				_movementIndex[i,n] = 1;
			}
		}

		for (int i = 0; i < _movementIndex.GetLength(0); i++) {
			// Convert i to bits
			BitArray bits = new BitArray (new int[] {i});
			BitArray id = new BitArray(8);

			for (int j = 0; j < id.Length; j++) {
				id[j] = bits[j];
			}
			// Reversing bitArray
			Reverse (id);

			for (int n = 0; n < _movementIndex.GetLength(1); n++) {
				// Populate allowed paths depends on movement type
				// If ID is false and _movementIndex == 1
				if (!id[n] && _movementIndex[i,n] == 1) {
					_movementIndex[i,n] = 0;

					if (n%2 != 0 ) {
						_movementIndex[i,n - 1] = 0;

						if (n == 7) {
							_movementIndex[i, 0] = 0;
						} else {
							_movementIndex[i, n + 1] = 0;
						}
					}
				}
			}
		}
	}
	
	void Reverse(BitArray array)
	{
		int length = array.Length;
		int mid = (length / 2);
		
		for (int i = 0; i < mid; i++)
		{
			bool bit = array[i];
			array[i] = array[length - i - 1];
			array[length - i - 1] = bit;
		}    
	}
	
	// Calculate new path
	public List<Node> FindPath (Node _source, Node _target) {
		if (_source == _target) {
			Debug.LogWarning ("Pathfinding: source == target");
			return null;
		}
		
		Dictionary<Node, float> dist = new Dictionary<Node, float> ();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();
		
		// Setup the "unvisited" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node> ();
		
		dist [_source] = 0;
		prev [_source] = null;
		
		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach (GameObject tile in NodeMap) {
			Node v = tile.GetComponent<Node>();
			if (v != _source) {
				dist [v] = Mathf.Infinity;
				prev [v] = null;
			}
			unvisited.Add(v);
		}
		
		while (unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;
			
			foreach (Node possibleU in unvisited) {
				if (u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}
			
			if (u == _target) {
				break;
			}
			
			unvisited.Remove (u);

			// For every neighbour of Unvisited node
			for (int i = 0; i < u.neighbours.Length; i++) {
				if (u.neighbours[i] != null) {

					// V - it's current neighbour
					Node v = u.neighbours[i];
					// Alternate coast is previous coast + of Unvisited node + path to the neighbour
					float alt = dist[u] + TileCoast(u, v, i);

					if (alt < dist[v]) {
						// Alternate coast is less then previous neighbour coast
						// Apply alternate coast to the neighbour
						dist[v] = alt;
						prev[v] = u;
					}
				}
			}
		}
		
		
		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.
		if (prev[_target] == null) {
			
			// No route between our target and the source
			// Try to find closest node
			Node closestNode = null;
			
			int sourceLengthX = Mathf.Abs(_source.x - _target.x);
			int sourceLengthY = Mathf.Abs(_source.y - _target.y);
			int sourceLength = sourceLengthX + sourceLengthY;
			
			int shortestLength = sourceLength;
			
			// Try to find closest node from available
			foreach (var c in prev) {
				if (c.Value != null) {
					int lengthX = Mathf.Abs(c.Key.x - _target.x);
					int lengthY = Mathf.Abs(c.Key.y - _target.y);
					int lengthSumm = lengthX + lengthY;
					
					if (shortestLength > lengthSumm) {
						closestNode = c.Key;
						shortestLength = lengthSumm;
					}
				}
			}
			
			if (shortestLength < sourceLength) {
				_target = closestNode;
			} else {
				return null;
			}
			
		}
		
		List<Node> currentPath = new List<Node> ();
		Node curr = _target;
		
		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}
		
		currentPath.Reverse ();
		return currentPath;
	}

	// Tile Coast
	float TileCoast(Node _source, Node _target, int _direction) {
		//Base coast
		float coast = 1;

		if (_target.DirectionLayers [0] == -1) {
			coast = Mathf.Infinity;
		}

		int map = 0;
		if (_source.DirectionLayers [0] == -1) {
			map = CalculateNodeMask (_source, 0);
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

	public Node GetRandomPlace () {
		Node node;

		do {
			node = null;
			int x;
			int y;

			x = UnityEngine.Random.Range (0, NodeMap.GetLength(0));
			y = UnityEngine.Random.Range (0, NodeMap.GetLength(1));

			if (NodeMap [x, y].GetComponent<Node>().DirectionLayers[0] != -1) {
				node = NodeMap [x, y].GetComponent<Node>();
			}
		}
		while (node == null);

		return node;
	}

	public void PlaceUnit (GameObject unitPrefab) {
		GameObject unitObject = (GameObject) GameObject.Instantiate (unitPrefab);

		Unit unitObjectController = unitObject.GetComponent<Unit> ();
		unitObjectController.Main = Main;
		unitObjectController.Place (GetRandomPlace ());
	}
}
