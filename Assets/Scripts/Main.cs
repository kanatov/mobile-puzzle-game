using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Main : MonoBehaviour {

	//Public data
	public Map Map;
	public GameObject selectedObject;
	public GameObject MapRootPrefab;
	public GameObject NodePrefab;
	public GameObject[] TerrainPrefab;
	public UnitType[] UnitTypes;
	
	// Level properties
	int unitCount = 50;
	int mapWidth = 20;
	int mapHeight = 20;

	void Start () {
		Map = new Map(this.GetComponent<Main>(), MapRootPrefab, NodePrefab, mapWidth, mapHeight);

		for (int i = 0; i < unitCount; i++) {
			Map.PlaceUnit (UnitTypes [0].gameObject);
		}
	}	

	void Update () {
		for (int x = 0; x < Map.NodeMap.GetLength (0); x++) {
			for (int y = 0; y < Map.NodeMap.GetLength (1); y++) {
				Node nodes = Map.NodeMap[x, y].GetComponent<Node>();
				if (nodes.terrain == 0) {
					if (nodes.DirectionLayers[0] != -1) {
						nodes.Normal();
					} else {
						nodes.Highlite (0.6f, 0.6f, 0.3f);
					}
				}
			}
		}
	}

	// Tile > World coordinates converter
	public Vector3 GetWorldCoordinates (Node _node) {
		return new Vector3 (_node.x, 0f, _node.y);
	}

	public void TileClick(Node _target) {
		if (selectedObject) {
			selectedObject.GetComponent<Unit>().GoTo(_target);
		}
	}

	public void Select(GameObject _selected) {
		if (selectedObject == _selected) {
			selectedObject = null;
			_selected.GetComponent<Unit>().Deselect();
		} else {
			if (selectedObject != null) {
				selectedObject.GetComponent<Unit>().Deselect();
			}
			selectedObject = _selected;
			_selected.GetComponent<Unit>().Select();
		}
	}
}
