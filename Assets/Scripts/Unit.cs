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
	GameObject AttackTarget;

	public Cell stepTarget;
	public Color highliteColor = Color.blue;
	public Color normalColor;

	// Private data
	GameObject model;

	Cell target;

	List<Cell> path = null;
	List<Cell> pathDebug = new List<Cell> ();

	bool timerLock = false;
	int counter = 0;
	int updatePath = 10;
	int DistanceToTarget = 0;

	// Debug
	Color debugPathColor = Color.yellow;


	// TODO AI: relation between units: units go through each other or shifting if it's only the one way
	// TODO AI: Unit run away from zombies
	// TODO AI: imidietly find new path if obstacle (once)
	// TODO Pathf: One cell step: Simple A* pathfinding
	// TODO Pathf: Convert cells graph to nodes
	// TODO Pathf: Small grid
	// TODO Pathf: move by cell coordinates
	// TODO AI: Slow unit follow to the fast
	// TO DO Movement out of grid


	public void Init (int _type, Cell _pos)
	{
		Type = _type;
		target = stepTarget = _pos;
		this.GetComponent<Transform> ().position = Map.GetWorldCoordinates (_pos);

		model = (GameObject)Instantiate (UnitTypes [_type].model);
		model.GetComponent<Transform> ().SetParent (this.GetComponent<Transform> ());
		model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0.5f, 0f);
		normalColor = model.GetComponent<Renderer> ().material.color;

		Map.UpdateCellMask (stepTarget, 1, -1);
	}

	void Update ()
	{
		if (GetComponent<Transform> ().position == Map.GetWorldCoordinates (stepTarget)) {
			if (Type == 0)
				MakeStep();

			if (Type == 1)
				EnemyAction ();
		} else {
			WalkAnimation ();
			DebugDrawPath();
		}
	}

	void WalkAnimation()
	{
		GetComponent<Transform> ().position = Vector3.MoveTowards (
			GetComponent<Transform> ().position,
			Map.GetWorldCoordinates (stepTarget),
			Time.deltaTime * UnitTypes [Type].speed
			);
	}

	void EnemyAction() {
		UpdateTarget();

		if (AttackTarget != null) {
			if (DistanceToTarget < 20) {
				AttackTarget.GetComponent<Unit>().Die();
				Map.PlaceUnit(0);
				return;
			}
		}

		if (timerLock) {
			MakeStep();
		} else {
			GoTo (target);
		}

	}

	void UpdateTarget() {
		DistanceToTarget = 0;
		target = stepTarget;
		AttackTarget = null;

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		foreach (var player in players) {
			Unit unit = player.GetComponent<Unit>();
			
			int tmpDeistance = Map.GetDistance (stepTarget, unit.stepTarget);
			// TODO if distance < attack → break
			
			if (tmpDeistance < DistanceToTarget || DistanceToTarget == 0) {
				DistanceToTarget = tmpDeistance;
				target = unit.stepTarget;
				AttackTarget = player;
			}
		}
	}

	void MakeStep() {
		if (path == null) {
			if (target == stepTarget) {
				// Our journey is finished let's get a new journey
				pathDebug = new List<Cell> ();
				GoTo (Map.GetRandomPlace ());
			} else {
				// No way to our target, try to find new way
				GoTo (target);
			}
			
			return;
		}
		
		if (path.Count == 1) {
			path = null;
			return;
		}

		if (path [1].DirectionLayers [1] == -1) {
			// The next cell is unavailable
			// TODO AI: after pause try again same path

			if (!timerLock)
				path = null;

			return;
		}

		Map.UpdateCellMask (path [0], 1, 255);
		stepTarget = path [1];
		Map.UpdateCellMask (stepTarget, 1, -1);
		path.RemoveAt (0);
	}

	public void GoTo (Cell _target)
	{
		if (!timerLock) {
			timerLock = true;
			Invoke ("TimerUnlock", 0.5f);

			target = _target;
			path = Map.FindPath (stepTarget, target);

			if (path != null) {
				// Debug
				pathDebug = new List<Cell> (path);
				debugPathColor = Color.yellow;

				// Game
				target = path.Last();
				MakeStep();
			} else {
				// Debug
				pathDebug = new List<Cell> ();
				pathDebug.Add (stepTarget);
				pathDebug.Add (target);
				debugPathColor = Color.red;

				// Game
				GoTo (target);
			}
		}
	}
	
	void TimerUnlock ()
	{
		timerLock = false;
	}

	void OnMouseUp ()
	{
		Main.Select (this.GetComponent<Unit> ());
	}

	public void Select ()
	{
		model.GetComponent<Renderer> ().material.color = highliteColor;
	}
	
	public void Deselect ()
	{
		model.GetComponent<Renderer> ().material.color = normalColor;
	}

	public void Die() {
		Map.UpdateCellMask (stepTarget, 1, 255);
		Destroy(gameObject);
	}

	void DebugDrawPath(){
		int currCell = 0;
		
		while (currCell < pathDebug.Count - 1) {
			Cell startCell = pathDebug [currCell];
			Vector3 start = Map.GetWorldCoordinates (startCell);
			
			Cell endCell = pathDebug [currCell + 1];
			Vector3 end = Map.GetWorldCoordinates (endCell);
			
			Debug.DrawLine (start, end, debugPathColor);
			
			currCell++;
		}
	}
}
