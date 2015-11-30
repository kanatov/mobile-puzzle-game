using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Terrain {
	public static float offsetX, offsetY;

	public static void Init () {
		float unitLength = 0.5f / (Mathf.Sqrt(3)/2);
		
		offsetX = unitLength * 1.5f;
		offsetY = unitLength * Mathf.Sqrt(3);
	}

	public static void SetTerrain (Cell _cell) {
		GameObject model;

		if (_cell.y % 2 == 0) {
			model = (GameObject)GameObject.Instantiate (Map.TerrainModels [0]);
		} else {
			model = (GameObject)GameObject.Instantiate (Map.TerrainModels [1]);
		}
		model.GetComponent<Transform> ().SetParent (_cell.GetComponent<Transform> ());
		model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0f, 0f);
		model.GetComponent<Transform> ().localScale = new Vector3 (1f, 1f, 1f);
		
		_cell.model = model;
	}
}
