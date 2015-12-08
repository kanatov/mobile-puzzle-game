using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cell {
	[System.NonSerialized]
	public GameObject terrainModel;
	public int unitsAndItems;
	public Unit unit;
}
