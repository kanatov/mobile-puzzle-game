using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cell {
	public Cell[] neighbours = new Cell[6];
	public int unitsAndItems;
	public int x; // HACK
	public int y;
	public int z;

	[System.NonSerialized]
	public GameObject terrainModel;
}
