using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Unit : MonoBehaviour
{
	// Global unit class
	// Public data
	public Main Main;
	public Map Map;
	// Direction group depend the allowed direction layer
	// 0. Player
	// 1. Zombie
	public int Type;
	public UnitType[] UnitTypes;

	public Cell stepTarget;

	// Private data
	GameObject model;

	Cell target;

	List<Cell> path = null;
	List<Cell> pathDebug = new List<Cell> ();

	bool go = true;
	int counter = 0;
	int updatePath = 10;

	// Debug
	Color debugPathColor = Color.yellow;

	public void Init (int _type, Cell _pos)
	{
		Type = _type;
		target = stepTarget = _pos;
		this.GetComponent<Transform> ().position = Map.GetWorldCoordinates (_pos);

		model = (GameObject)Instantiate (UnitTypes [_type].model);
		model.GetComponent<Transform> ().SetParent (this.GetComponent<Transform> ());
		model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0.5f, 0f);

		Map.UpdateCellMask (stepTarget, 0, -1);
	}

	void Update ()
	{
		// Unit animation completed
		if (go && GetComponent<Transform> ().position == Map.GetWorldCoordinates (stepTarget)) {
			GetStep ();
			return;
		}

		// Debug Path
		int currCell = 0;
		
		while (currCell < pathDebug.Count - 1) {
			Cell startCell = pathDebug [currCell];
			Vector3 start = Map.GetWorldCoordinates (startCell);
			
			Cell endCell = pathDebug [currCell + 1];
			Vector3 end = Map.GetWorldCoordinates (endCell);
			
			Debug.DrawLine (start, end, debugPathColor);
			
			currCell++;
		}

		// Walk animation
		GetComponent<Transform> ().position = Vector3.MoveTowards (
			GetComponent<Transform> ().position,
			Map.GetWorldCoordinates (stepTarget),
			Time.deltaTime * UnitTypes [Type].speed
		);
	}

	// TODO AI: relation between units: units go through each other or shifting if it's only the one way
	// TODO AI: Unit run away from zombies
	// TODO AI: imidietly find new path if obstacle (once)
	// TODO Pathf: One cell step: Simple A* pathfinding
	// TODO Pathf: Convert cells graph to nodes
	// TODO Pathf: Small grid
	// TODO Pathf: move by cell coordinates
	
	// TO DO Movement out of grid

	void GetStep ()
	{
		if (path == null) {
			if (target == stepTarget) {
				pathDebug = new List<Cell> ();
				GoTo (Map.GetRandomPlace ());
			} else {
				Invoke ("TryGoTo", 1);
			}

			return;
		}

		if (path.Count == 1) {
			path = null;
			return;
		}
			
		if (path [1].DirectionLayers [UnitTypes [0].DirectionGroup] == -1) {
			// The next cell is unavailable
			path = null;
			return;
		}
			
		if (target != stepTarget) {
			// Unit target is not achived yet
			// Making step
				
			if (counter < updatePath) {
				counter++;
				Map.UpdateCellMask (path [0], 0, 255);

//					if (Type == 1) {
//						// If unit is Zombie: check closest player unit
//						FindClosestPlayer ();
//						return;
//					}

				stepTarget = path [1];
				Map.UpdateCellMask (stepTarget, 0, -1);
				path.RemoveAt (0);
			} else {
				GetRandomUpdate ();
				GoTo (target);
			}
		} else {
			path = null;
		}
	}

	public void GoTo (Cell _target)
	{
		if (stepTarget != _target) {

			target = _target;
			counter = 0;

			path = Map.FindPath (stepTarget, target);

			if (path != null) {
				// Debug
				pathDebug = new List<Cell> (path);
				debugPathColor = Color.yellow;
				// Game
				go = true;
			} else {
				// Debug
				pathDebug = new List<Cell> ();
				pathDebug.Add (stepTarget);
				pathDebug.Add (target);
				debugPathColor = Color.red;

				// Game
				go = false;

				Invoke ("TryGoTo", 1);
			}
		}
	}

	void TryGoTo ()
	{
		GoTo (target);
	}

	void GetRandomUpdate ()
	{
		updatePath = Random.Range (10, 20);
	}

	void FindClosestPlayer ()
	{
		Cell altTarget = null;
		int altDistance = 0;
		
		foreach (var player in Main.Players) {
			int tmpDeistance = Map.GetDistance (stepTarget, player.stepTarget);
			if (tmpDeistance < altDistance || altDistance == 0) {
				altDistance = tmpDeistance;
				altTarget = player.stepTarget;
			}
		}

		path = null;
		GoTo (altTarget);
		
	}

	void OnMouseUp ()
	{
		Main.Select (this.GetComponent<Unit> ());
	}
	
	public void Select ()
	{
		model.GetComponent<Renderer> ().material.color = Color.red;
	}
	
	public void Deselect ()
	{
		model.GetComponent<Renderer> ().material.color = Color.white;
	}
}
