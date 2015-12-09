using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cell {
	[System.NonSerialized]
	public GameObject terrainModel;
	public Cell[] neighbours = new Cell[6];
	public int unitsAndItems;
	// HACK
	public int x;
	public int y;
}
