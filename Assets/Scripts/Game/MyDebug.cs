using UnityEngine;
using System.Collections;
using GenericData;

public static class MyDebug {
	public static void ClearData () {
		SaveLoad.Delete(SaveLoad.nameGame);
		SaveLoad.Delete(SaveLoad.namePlayerProgress);
		SaveLoad.Delete(SaveLoad.nameUnits);
	}

//	public static void DebugDrawClosedCells ()
//	{
//		for (int x = 0; x < Cells.GetLength (0); x++) {
//			for (int y = 0; y < Cells.GetLength (1); y++) {
//				if (Cells [x, y].terrain == 0) {
//					if (Cells [x, y].DirectionLayers [1] != -1) {
//						DebugNormalTile(Cells [x, y]);
//					} else {
//						DebugHighliteTile(Cells [x, y], 0.6f, 0.6f, 0.3f);
//					}
//				}
//			}
//		}
//	}
	
//	public static void DebugHighliteTile (Cell _cell, float _r, float _g, float _b)
//	{
//		GameObject tile = _cell.Tile;
//		
//		if (tile != null) {
//			if (tile.GetComponent<Renderer> ().material.color != _cell.highliteColor) {
//				tile.GetComponent<Renderer> ().material.color = new Color (_r, _g, _b);
//			}
//		}
//	}
//	
//	public static void DebugNormalTile (Cell _cell)
//	{
//		GameObject tile = _cell.Tile;
//		
//		if (tile != null) {
//			tile.GetComponent<Renderer> ().material.color = _cell.normalColor;
//		}
//	}
}
