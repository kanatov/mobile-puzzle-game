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

	Unit AttackTarget;
	int AttackTargetDistance;
	public List<Unit> Enemyes = new List<Unit>();

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

	bool die = false;

	// Debug
	Color debugPathColor = Color.yellow;


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
		if (Type == 0)
			Main.Players.Add(this);

		if (Type == 1)
			Main.Enemies.Add(this);
	}

	void Update ()
	{
		if (GetComponent<Transform> ().position == Map.GetWorldCoordinates (stepTarget)) {
			if (Type == 0)
				PlayerAction ();

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
				return;
			}
		}

		if (timerLock) {
			MakeStep();
		} else {
			GoTo (target);
		}

	}

	void PlayerAction() {

	}

	void UpdateTarget() {
		DistanceToTarget = 0;
		target = stepTarget;
		AttackTarget = null;

		foreach (var player in Main.Players) {
			Unit unit = player;
			
			int tmpDistance = Map.GetDistance (stepTarget, unit.stepTarget);
			// TODO if distance < attack → break
			
			if (tmpDistance < DistanceToTarget || DistanceToTarget == 0) {
				DistanceToTarget = tmpDistance;
				target = unit.stepTarget;
				AttackTarget = player;
			}
		}

//		if (!AttackTarget.GetComponent<Unit>().Enemyes.Contains(this)) {
//			AttackTarget.GetComponent<Unit>().Enemyes.Add(this);
//		}

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
			Invoke ("TimerUnlock", 1f);

			if (_target != stepTarget) {
				target = _target;
				path = Map.FindPath (stepTarget, target);
			}

//			Debug.Log ("Path: " + path.Count + ", Source: " + stepTarget.x + " " + stepTarget.y + ", Target: " + target.x + " " + target.y);

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
		if (!die ) {
			die = true;
			if (Type == 0) {
				Main.Players.Remove(this);
				Map.PlaceUnit(0);
			}

			if (Type == 1) {
				Main.Enemies.Remove(this);
				Map.PlaceUnit(1);
			}

			Map.UpdateCellMask (stepTarget, 1, 255);
			Destroy(gameObject);
		}
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
