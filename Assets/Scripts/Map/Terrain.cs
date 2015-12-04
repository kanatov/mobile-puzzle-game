using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Terrain {
	public static void GetTerrain (Cell _cell) {
		GameObject model;

		if (_cell.y % 2 == 0) {
			model = (GameObject)GameObject.Instantiate (Map.terrainModels [0]);
		} else {
			model = (GameObject)GameObject.Instantiate (Map.terrainModels [1]);
		}
		model.GetComponent<Transform> ().SetParent (_cell.GetComponent<Transform> ());
		model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0f, 0f);
		model.GetComponent<Transform> ().localScale = new Vector3 (1f, 1f, 1f);
		
		_cell.model = model;
	}

	public static int GetUnit (Cell _cell) {
		if (_cell.x + _cell.y != 0 && _cell.x % 6 == 0 && _cell.y % 6 == 0) {
			return 0;
		}

		return -1;
	}
}
