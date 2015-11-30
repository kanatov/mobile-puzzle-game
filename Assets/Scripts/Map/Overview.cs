using UnityEngine;
using System.Collections;

public class Overview {
	public static bool shift = true;

	static int[,] overviewMask5 = new int[,] {
		{0,1,1,1,0},
		{0,1,1,1,1},
		{1,1,1,1,1},
		{0,1,1,1,1},
		{0,1,1,1,0}
	};
	
	static int[,] overviewMask7 = new int[,] {
		{0,1,1,1,1,0,0},
		{0,1,1,1,1,1,0},
		{1,1,1,1,1,1,0},
		{1,1,1,1,1,1,1},
		{1,1,1,1,1,1,0},
		{0,1,1,1,1,1,0},
		{0,1,1,1,1,0,0}
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
	
	public static int[][,] overviewMasks = new int [][,] {overviewMask5, overviewMask7, overviewMask9};
	public static int[][,] overviewMasksShift = new int [overviewMasks.Length][,];

	public static void Init() {
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

	public static int[,] Get (int _size) {
		if (shift) {
			return overviewMasks[_size];
		} else {
			return overviewMasksShift[_size];
		}
	}
}
