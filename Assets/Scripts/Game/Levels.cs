using UnityEngine;
using System.Collections.Generic;

public static class Levels {
	static int[,] terrain0 = new int[,] {
		{1,1,1,1,1,1,1,1,1,1,1,1,1},
		{1,0,1,1,1,0,0,0,0,0,0,0,1},
		{1,0,0,1,0,0,1,1,0,0,1,0,1},
		{1,1,0,0,0,0,1,1,1,0,0,0,1},
		{1,0,0,1,0,0,1,1,0,0,1,0,1},
		{1,0,1,1,1,0,0,0,0,0,0,0,1},
		{1,1,1,1,1,1,1,1,1,1,1,1,1}
	};
	public static int [][,] terrains = new int[][,] {terrain0};


	static int[,] unitsAndItems0 = new int[,] {
		{0,0,0,0,0,0,0,0,0,0,0,0,0},
		{0,0,0,0,0,0,0,0,0,0,0,0,0},
		{0,0,0,0,0,0,0,0,0,0,0,0,0},
		{0,0,0,0,0,0,0,0,0,0,0,1,0},
		{0,0,0,0,0,0,0,0,0,0,0,0,0},
		{0,0,0,0,0,0,0,0,0,0,0,0,0},
		{0,0,0,0,0,0,0,0,0,0,0,0,0}
	};
	public static int [][,] unitsAndItems = new int[][,] {unitsAndItems0};
}

