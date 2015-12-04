using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Terrain {
	static int e = 3;
	public static int GetTerrain (Cell _cell) {
		if (_cell.y % 2 == 0) {
			return 1;
		} else {
			return 0;
		}
	}


	public static int GetUnit (Cell _cell) {
		if (_cell.x == Player.x && _cell.y == Player.y) {
			return 0;
		}

		if (_cell.x + _cell.y != 0 && _cell.x % e == 0 && _cell.y % e == 0) {
			return 1;
		}

		return -1;
	}
}
