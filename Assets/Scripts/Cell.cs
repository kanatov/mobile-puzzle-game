using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour, IHeapItem<Cell>
{
	public Cell[] neighbours = new Cell[8];
	public Main Main;
	public int terrain;
	public int x;
	public int y;
	public Color highliteColor = Color.yellow;
	public Color normalColor;
	public GameObject Tile;
	public Cell parent;
	public int gCost;
	public int hCost;
	public int[] DirectionLayers;
	public Material material;

	int heapIndex;

	void Update () {
		Fade();
	}

	void OnMouseUp ()
	{
		if (terrain == 0) {
			material.color = highliteColor;
			UnitManager.SetCellTarget (this);
		}
	}

	void Fade() {
		if (material.color != normalColor) {
			material.color = Color.Lerp (material.color, normalColor, 0.1f);
		}
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}

		set {
			heapIndex = value;
		}

	}

	public int CompareTo (Cell cellToCompare) {
		int compare = fCost.CompareTo(cellToCompare.fCost);

		if (compare == 0) {
			compare = hCost.CompareTo(cellToCompare.hCost);
		}

		return -compare;
	}
}


