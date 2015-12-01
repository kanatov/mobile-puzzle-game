using UnityEngine;

public static class Units {
	// Public
	public static UnitType[] unitTypes;
	public static GameObject unitContainer;

	public static void GetUnit (Cell _cell) {
		int id = Terrain.GetUnit(_cell);

		if (id == -1) {
			return;
		}

		GameObject unitObject = (GameObject)GameObject.Instantiate (unitContainer);
		unitObject.GetComponent<Transform> ().SetParent (_cell.GetComponent<Transform> ());
		unitObject.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0f, 0f);
		unitObject.GetComponent<Transform> ().localScale = new Vector3 (1f, 1f, 1f);
		Unit unit = unitObject.GetComponent<Unit> ();
		unit.id = id;

		// Look
		unit.model = GameObject.Instantiate (unitTypes[unit.id].model);
		unit.model.GetComponent<Transform> ().SetParent (unit.GetComponent<Transform> ());
		unit.model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0.5f, 0f);
		unit.model.GetComponent<Transform> ().localScale = new Vector3 (1f, 1f, 1f);

		// Attack
		unit.attackDistance = unitTypes[unit.id].attackDistance;
		unit.viewDistance = unitTypes[unit.id].viewDistance;

		// Health
		unit.dead = false;
	}
}

