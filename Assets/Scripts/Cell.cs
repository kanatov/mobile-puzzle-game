using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour, IHeapItem<Cell>
{
	// Neighbours connections to each direction
	public Cell[] neighbours = new Cell[8];
	public Main Main;
	public int terrain;
	public int x;
	public int y;
	public Color highliteColor = Color.blue;
	public Color normalColor;
	public GameObject Tile;
	public Cell parent;
	public int gCost;
	public int hCost;

	// DirectionLayerss of cell emptyness
	// Compare index
	// 0. Ground
	// 1. Units
	public int[] DirectionLayers;
	int heapIndex;

	void OnMouseUp ()
	{
		Main.CellClick (this.GetComponent<Cell> ());
	}

	public void Highlite (float _r, float _g, float _b)
	{
		if (Tile != null) {
			if (Tile.GetComponent<Renderer> ().material.color != highliteColor) {
				Tile.GetComponent<Renderer> ().material.color = new Color (_r, _g, _b);
			}
		}
	}

	public void Normal ()
	{
		if (Tile != null) {
			Tile.GetComponent<Renderer> ().material.color = normalColor;
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


