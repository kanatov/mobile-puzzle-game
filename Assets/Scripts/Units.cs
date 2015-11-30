using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// TODO AI: relation between units: units go through each other or shifting if it's only the one way
// TODO AI: Unit run away from zombies
// TODO AI: imidietly find new path if obstacle (once)
// TODO Pathf: One cell step: Simple A* pathfinding
// TODO Pathf: Convert cells graph to nodes
// TODO Pathf: Small grid
// TODO Pathf: move by cell coordinates
// TODO Pathf: follow by player path
// TODO AI: Slow unit follow to the fast
// TODO AI: If enemy far away from the player – mage group of enemies and round the player
// TODO Calculate only heighbour chunks

public static class UnitManager {

	// Public
	public static List<Unit> Players = new List<Unit>();
	public static List<Unit> Enemies = new List<Unit>();
	public static UnitType[] UnitTypes;
	public static GameObject UnitContainer;
	public static GameObject[] UI;

	// Private
	static Color highliteColor = new Color(1f, 0.5f, 0f);

	public static void Create (int _id) {
		GameObject unitContainer = (GameObject)GameObject.Instantiate (UnitContainer);
		Unit unit = unitContainer.GetComponent<Unit> ();

		// Type of the Unit
		unit.id = _id;

		// Position
		unit.speed = UnitTypes[unit.id].speed;
		unit.directionLayer = UnitTypes[unit.id].directionLayer;

		unit.source = MapManager.GetRandomPlace ();
		MapManager.UpdateCellMask (unit.source, unit.directionLayer, false);
		unit.GetComponent<Transform> ().position = MapManager.GetWorldCoordinates (unit.source);

		// Look
		unit.model = GameObject.Instantiate (UnitTypes[unit.id].model);
		unit.model.GetComponent<Transform> ().SetParent (unit.GetComponent<Transform> ());
		unit.model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0.5f, 0f);

		unit.material = unit.model.GetComponent<Renderer>().material;
		unit.normalColor = unit.material.color;
		
		// Attack
		unit.attackDistance = UnitTypes[unit.id].attackDistance;
		unit.viewDistance = UnitTypes[unit.id].viewDistance;

		// Health
		unit.dead = false;

		if (unit.id == 0) {
			Players.Add(unit);
		} else {
			Enemies.Add(unit);
		}
	}
}

