using UnityEngine;
using System.Collections;

public class Overview {
	public static bool shift;

	static int[,] overviewMask5 = new int[,] {
		{0,1,1,1,0},
		{0,1,1,1,1},
		{1,1,1,1,1},
		{0,1,1,1,1},
		{0,1,1,1,0}
	};
	
	static int[,] overviewMask7 = new int[,] {
		{0,0,1,1,1,1,0},
		{0,1,1,1,1,1,0},
		{0,1,1,1,1,1,1},
		{1,1,1,1,1,1,1},
		{0,1,1,1,1,1,1},
		{0,1,1,1,1,1,0},
		{0,0,1,1,1,1,0}
	};
	
	static int[,] overviewMask9 = new int[,] {
		{0,0,1,1,1,1,1,0,0},
		{0,0,1,1,1,1,1,1,0},
		{0,1,1,1,1,1,1,1,0},
		{0,1,1,1,1,1,1,1,1},
		{1,1,1,1,1,1,1,1,1},
		{0,1,1,1,1,1,1,1,1},
		{0,1,1,1,1,1,1,1,0},
		{0,0,1,1,1,1,1,1,0},
		{0,0,1,1,1,1,1,0,0}
	};

	static int[,] overviewMask11 = new int[,] {
		{0,0,0,1,1,1,1,1,1,0,0}, // 5 3 6
		{0,0,1,1,1,1,1,1,1,0,0}, // 4 2 7
		{0,0,1,1,1,1,1,1,1,1,0}, // 3 2 8
		{0,1,1,1,1,1,1,1,1,1,0}, // 2 1 9
		{0,1,1,1,1,1,1,1,1,1,1}, // 1 1 10
		{1,1,1,1,1,1,1,1,1,1,1}, // 0 0 11
		{0,1,1,1,1,1,1,1,1,1,1},
		{0,1,1,1,1,1,1,1,1,1,0},
		{0,0,1,1,1,1,1,1,1,1,0},
		{0,0,1,1,1,1,1,1,1,0,0},
		{0,0,0,1,1,1,1,1,1,0,0}
	};

	// find the middle - 1
	// length - middle
	// the offset in the middle == 0


	public static int[][,] overviewMasks = new int [][,] {overviewMask5, overviewMask7, overviewMask9, overviewMask11};
	public static int[][,] overviewMasksShift = new int [overviewMasks.Length][,];

	public static void Init() {
		shift = true;

		for (int i = 0; i < overviewMasksShift.Length; i++) {
			int[,] newArray = new int[overviewMasks[i].GetLength(0),overviewMasks[i].GetLength(1)];

			for (int x = 0; x < overviewMasks[i].GetLength(0); x++) {
				for (int y = overviewMasks[i].GetLength(1); y > 0 ; y--) {
					newArray[x,overviewMasks[i].GetLength(1) - y] = overviewMasks[i][x,y-1];
				}
			}
			overviewMasksShift[i] = newArray;
		}
	}

	public static int[,] GetMask (int _size) {
		if (shift) {
			return overviewMasks[_size];
		} else {
			return overviewMasksShift[_size];
		}
	}

//	public static bool GetShift {
//		get {
//			if (Player.overview % 2 == 0) {
//				return !shift;
//			} else {
//				return shift;
//			}
//		}
//	}

}
