using UnityEngine;
using System.Collections.Generic;

public static class Levels {
	public static int [] startRotation = new int[] {3};

	static int[,] terrain0 = new int[,] {
		{-1,1,-1,-1,1,1,1, 1,1,1,1,1,-1},
		{ 1,0, 1,-1,1,0,0, 0,0,0,0,0, 1},
		{ 1,0, 0, 1,0,0,1, 1,0,0,1,0, 1},
		{-1,1, 0, 0,0,0,1,-1,1,0,0,0, 1},
		{ 1,0, 0, 1,0,0,1, 1,0,0,1,0, 1},
		{ 1,0, 1,-1,1,0,0, 0,0,0,0,0, 1},
		{-1,1,-1,-1,1,1,1, 1,1,1,1,1,-1}
	};
	public static int [][,] terrains = new int[][,] {terrain0};


	static int[,] unitsAndItems0 = new int[,] {
		{-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
		{-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
		{-1, 1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
		{-1,-1,-1,-1,-1,-1,-1,-1,-1, 1,-1, 0,-1},
		{-1, 1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
		{-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
		{-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1}
	};
	public static int [][,] unitsAndItems = new int[][,] {unitsAndItems0};
}

