using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Pathfinding {
//	public static List<Cell> GetPath (Cell _source, Cell _target) {
//		if (_source == null || _target == null) {
//			Debug.LogWarning ("Pathfinding: source == " + _source + ", target == " + _target);
//			return null;
//		}
//		
//		if (_source == _target) {
//			Debug.LogWarning ("Pathfinding: source == target");
//			return null;
//		}
//		
//		Heap<Cell> opened = new Heap<Cell> (Map.Cells.GetLength(0) * Map.Cells.GetLength(1));
//		HashSet<Cell> closed = new HashSet<Cell> ();
//		
//		opened.Add (_source);
//		
//		// HACK
//		bool recover = false;
//		if (_target.DirectionLayers[0] == -1) {
//			Map.UpdateCellMask(_target, 0, true);
//			recover = true;
//		}
//		
//		while (opened.Count > 0) {
//			// Assign some active node as current
//			Cell currentCell = opened.RemoveFirst();
//			
//			closed.Add (currentCell);
//			
//			
//			if (currentCell == _target) {
//				// HACK
//				if (recover) {
//					Map.UpdateCellMask(_target, 0, false);
//				}
//				
//				break;
//			}
//			
//			// HACK
//			int map;
//			if (currentCell == _source) {
//				map = Map.CalculateCellMask (currentCell, 0);
//			} else {
//				map = currentCell.DirectionLayers [0];
//			}
//			
//			
//			// For every neighbour of current cell
//			for (int i = 0; i < currentCell.neighbours.Length; i++) {
//				// Check for an empty neighbour
//				if (currentCell.neighbours [i] == null)
//					continue;
//				
//				if (closed.Contains (currentCell.neighbours [i]))
//					continue;
//				
//				// Check for allowed direction
//				if (Map.AllowedDirections [map, i] == 0)
//					continue;
//				
//				int newMovementCostToNeghbour = currentCell.gCost + Map.GetDistance (currentCell, currentCell.neighbours [i]);
//				
//				if (newMovementCostToNeghbour < currentCell.neighbours [i].gCost || !opened.Contains (currentCell.neighbours [i])) {
//					currentCell.neighbours [i].gCost = newMovementCostToNeghbour;
//					currentCell.neighbours [i].hCost = Map.GetDistance (currentCell.neighbours [i], _target);
//					
//					currentCell.neighbours [i].parent = currentCell;
//					
//					if (!opened.Contains (currentCell.neighbours [i]))
//						opened.Add (currentCell.neighbours [i]);
//				}
//			}
//		}
//		
//		if (!closed.Contains (_target)) {
//			Debug.LogWarning ("Unreacheble goal! " + _source.x + " " + _source.y + ", " + _target.x + " " + _target.y + ". " + Time.timeSinceLevelLoad);
//			Cell closestCell = _source;
//			int distance = Map.GetDistance (_source, _target);
//			
//			foreach (var cell in closed) {
//				int altDistance = Map.GetDistance(cell, _target);
//				if (altDistance < distance){
//					closestCell = cell;
//					distance = altDistance;
//				}
//			}
//			
//			if (closestCell == _source) {
//				return null;
//			} else {
//				_target = closestCell;
//			}
//		}
//		
//		List<Cell> path = new List<Cell> ();
//		Cell tmpCell = _target;
//		
//		while (tmpCell != _source) {
//			path.Add (tmpCell);
//			tmpCell = tmpCell.parent;
//		}
//		path.Add (_source);
//		
//		path.Reverse ();
//		return path;
//	}
}
