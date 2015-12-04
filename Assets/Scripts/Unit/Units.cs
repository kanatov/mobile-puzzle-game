using UnityEngine;
using System.Collections.Generic;

public static class Units {
	// Public
	public static UnitType[] unitTypes;
	public static GameObject unitContainer;
	public static List<GameObject> enemies;
	static int enemyYShift = 1;

	public static void GetUnit (Cell _cell) {
		int id = Terrain.GetUnit(_cell);

		if (id == -1) {
			return;
		}

		GameObject unitObject = (GameObject)GameObject.Instantiate (unitContainer);
		unitObject.GetComponent<Transform> ().SetParent (_cell.GetComponent<Transform> ());
		unitObject.GetComponent<Transform>().localPosition = new Vector3 (0f, 0f, 0f);
		unitObject.GetComponent<Transform>().localScale = new Vector3 (1f, 1f, 1f);

		Unit unit = unitObject.GetComponent<Unit> ();
		unit.id = id;

		// Look
		unit.model = GameObject.Instantiate (unitTypes[unit.id].model);
		unit.model.GetComponent<Transform> ().SetParent (unit.GetComponent<Transform> ());
		unit.model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0.5f, 0f);
		unit.model.GetComponent<Transform> ().localScale = new Vector3(1f, 1f, 1f);

		// Attack
		unit.attackDistance = unitTypes[unit.id].attackDistance;
		unit.viewDistance = unitTypes[unit.id].viewDistance;

		// Health
		unit.dead = false;
	}

	public static void EnemyBehaviour (Unit _unit) {
		Transform unitTrans = _unit.GetComponent<Transform>();
		Cell source = unitTrans.parent.GetComponent<Cell>();

		// Check for an attack
		// Check for an obstackle
		// Check for a unit
		int hexShift = source.x % 2 == 0 ? enemyYShift : 0;

		List<Cell> targets = new List<Cell>();
		targets.Add (Map.GetCell(source.arrayX, source.arrayY + enemyYShift));
		targets.Add (Map.GetCell(source.arrayX - 1, source.arrayY + hexShift));
		targets.Add (Map.GetCell(source.arrayX + 1, source.arrayY + hexShift));

		int[] targetsOrder;

		if (source.x == Player.x) {
			targetsOrder = new int[] {0, 1, 2};
		} else if (source.x < Player.x) {
			targetsOrder = new int[] {2, 0, 1};
		} else {
			targetsOrder = new int[] {1, 0, 2};
		}
		
		foreach (var order in targetsOrder) {
			if (targets[order] == null) {
				continue;
			}

			Cell cell = targets[order];
				unitTrans.SetParent(cell.GetComponent<Transform> ());
			_unit.GetComponent<Move>().enabled = true;

			break;
		}
	}
}

