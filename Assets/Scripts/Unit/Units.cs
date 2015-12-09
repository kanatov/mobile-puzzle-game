using UnityEngine;
using System.Collections.Generic;

public static class Units {
	public static Unit GetUnit (Cell _cell) {
		Unit unit = new Unit();
		unit.id = _cell.unitsAndItems;
		unit.cell = _cell;

		return unit;
	}

	public static GameObject GetUnitContainer (Unit _unit) {
		// Unit container
		GameObject unitContainer = (GameObject)GameObject.Instantiate (GameController.unitContainer);
		unitContainer.GetComponent<Transform>().localPosition = new Vector3 (0f, 0f, 0f);
		
		// Unit model
		GameObject model = GameObject.Instantiate (UnitTypes.model[_unit.id]);
		model.GetComponent<Transform> ().SetParent (unitContainer.GetComponent<Transform>());
		model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 1f, 0f);

		return unitContainer;
	}

//	public static void Behaviour (Unit _unit, Direction _direction) {
//		// define list of cells
//		// compare list of enemies and cells and feedback
//		// walk list of cells and feedback
//		// error
//
//		switch (_direction) {
//		case Direction.Up:
////			Map.UpdateMap(0, 1);
//			break;
//		case Direction.Right:
////			Map.UpdateMap(1, 0);
//			break;
//		case Direction.Left:
////			Map.UpdateMap(-1, 0);
//			break;
//		}
//	}
//
////	public static void EnemyBehaviour (GameObject _unitContainer) {
////		// define list of cells
////		// compare list of enemies  and cells and feedback
////		// walk list of cells and feedback
////		// error
////
////		Transform unitTrans = _unitContainer.GetComponent<Transform>();
////		
////		Cell source = unitTrans.parent.GetComponent<Cell>();
////		int hexShift = source.x % 2 == 0 ? 1 : 0;
////		
////		List<Cell> targets = new List<Cell>();
////		targets.Add (Map.GetCell(source.arrayX, source.arrayY + 1));
////		targets.Add (Map.GetCell(source.arrayX - 1, source.arrayY + hexShift));
////		targets.Add (Map.GetCell(source.arrayX + 1, source.arrayY + hexShift));
////		targets.Add (Map.GetCell(source.arrayX, source.arrayY - 1));
////		targets.Add (Map.GetCell(source.arrayX - 1, source.arrayY));
////		targets.Add (Map.GetCell(source.arrayX + 1, source.arrayY));
////		
////		bool attack = false;
////		
////		foreach (var cell in targets) {
////			if (cell == null) {
////				continue;
////			}
////			
////			if (Player.x == cell.x && Player.y == cell.y) {
////				attack = true;
////				EnemyAttack (unitTrans.GetComponent<Unit>());
////				break;
////			}
////		}
////		
////		int[] targetsOrder;
////		
////		if (source.x == Player.x) {
////			targetsOrder = new int[] {0, 1, 2};
////		} else if (source.x < Player.x) {
////			targetsOrder = new int[] {2, 0, 1};
////		} else {
////			targetsOrder = new int[] {1, 0, 2};
////		}
////		
////		foreach (var order in targetsOrder) {
////			if (targets[order] == null) {
////				continue;
////			}
////			
////			if (targets[order].GetComponent<Transform>().childCount > 1) {
////				continue;
////			}
////			
////			Cell cell = targets[order];
////			unitTrans.SetParent(cell.GetComponent<Transform> ());
////			unitTrans.GetComponent<Unit>().GetComponent<Move>().enabled = true;
////			
////			break;
////		}
////	}
////
////	public static void EnemyAttack (Unit _unit) {
////		_unit.GetComponent<Transform>().localPosition = new Vector3 (0f, 0.5f, 0f);
////		_unit.GetComponent<Move>().enabled = true;
////		Player.health -= _unit.damage;
////		Debug.Log (Player.health);
////		if (Player.health <= 0) {
////			GameController.Init ();
////		}
////	}
}
