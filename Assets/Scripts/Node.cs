﻿using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour {
	// Neighbours connections to each direction
	public Node[] neighbours = new Node[8];
	public Main Main;
	public int terrain;
	public int x;
	public int y;
	public Color highliteColor = Color.blue;
	public Color normalColor;
	public GameObject Tile;

	// DirectionLayerss of cell emptyness
	// Compare index
	// 0. Ground (
	public int[] DirectionLayers;

	void OnMouseUp() {
		Main.TileClick (this.GetComponent<Node>());
	}

	public void Highlite(float _r, float _g, float _b) {
		if (Tile != null) {
			if (Tile.GetComponent<Renderer> ().material.color != highliteColor) {
				Tile.GetComponent<Renderer> ().material.color = new Color(_r,_g,_b);
			}
		}
	}

	public void Normal() {
		if (Tile != null) {
			Tile.GetComponent<Renderer> ().material.color = normalColor;
		}
	}
}


