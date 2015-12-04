using UnityEngine;
using System.Collections.Generic;

public static class Units {
	// Public
	public static UnitType[] unitTypes;
	public static GameObject unitContainer;
	public static List<GameObject> units;
	static int enemyYShift = 1;

	public static GameObject GetUnit (int _id) {
		GameObject _unitContainer = (GameObject)GameObject.Instantiate (unitContainer);
		_unitContainer.GetComponent<Transform>().localPosition = new Vector3 (0f, 0f, 0f);

		Unit unit = _unitContainer.GetComponent<Unit> ();
		unit.id = _id;

		// Look
		unit.model = GameObject.Instantiate (unitTypes[unit.id].model);
		unit.model.GetComponent<Transform> ().SetParent (unit.GetComponent<Transform> ());
		unit.model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 1f, 0f);

		// Attack
		unit.attackDistance = unitTypes[unit.id].attackDistance;
		unit.viewDistance = unitTypes[unit.id].viewDistance;
		unit.damage = unitTypes[unit.id].damage;

		// Health
		unit.dead = false;

		return _unitContainer;
	}

//	public static void Attack (Unit _unit) {
//		Transform unitTrans = _unit.GetComponent<Transform>();
//		Cell source = unitTrans.parent.GetComponent<Cell>();
//
//		int hexShift = source.x % 2 == 0 ? enemyYShift : 0;
//
//		List<Cell> targets = new List<Cell>();
//		targets.Add (Map.GetCell(source.arrayX, source.arrayY + enemyYShift));
//		targets.Add (Map.GetCell(source.arrayX - 1, source.arrayY + hexShift));
//		targets.Add (Map.GetCell(source.arrayX + 1, source.arrayY + hexShift));
//		targets.Add (Map.GetCell(source.arrayX, source.arrayY - enemyYShift));
//		targets.Add (Map.GetCell(source.arrayX - 1, source.arrayY));
//		targets.Add (Map.GetCell(source.arrayX + 1, source.arrayY));
//
//		bool attack = false;
//
//		foreach (var cell in targets) {
//			if (cell == null) {
//				continue;
//			}
//			cell.model.GetComponent<Renderer>().material.color = Color.gray;
//
//			if (Player.x == cell.x && Player.y == cell.y) {
//				attack = true;
//				EnemyAttack (_unit);
//				break;
//			}
//		}
//	}

//	public static void Walk (Unit _unit) {
//		int[] targetsOrder;
//
//		if (source.x == Player.x) {
//			targetsOrder = new int[] {0, 1, 2};
//		} else if (source.x < Player.x) {
//			targetsOrder = new int[] {2, 0, 1};
//		} else {
//			targetsOrder = new int[] {1, 0, 2};
//		}
//		
//		foreach (var order in targetsOrder) {
//			if (targets[order] == null) {
//				continue;
//			}
//
//			if (targets[order].GetComponent<Transform>().childCount > 1) {
//				continue;
//			}
//
//			Cell cell = targets[order];
//				unitTrans.SetParent(cell.GetComponent<Transform> ());
//			_unit.GetComponent<Move>().enabled = true;
//
//			break;
//		}
//	}

//	static void EnemyAttack (Unit _unit) {
//		_unit.GetComponent<Transform>().localPosition = new Vector3 (0f, 0.5f, 0f);
//		_unit.GetComponent<Move>().enabled = true;
//		Player.health -= _unit.damage;
//		Debug.Log (Player.health);
//		if (Player.health <= 0) {
//			GameController.Init ();
//		}
//	}
}

