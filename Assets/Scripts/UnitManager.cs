using UnityEngine;
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
// TO DO Movement out of grid

public static class UnitManager {
	// Public
	public static UnitType[] UnitTypes;
	public static GameObject UnitContainer;

	// Private
	public static List<Unit> Players = new List<Unit>();
	public static List<Unit> Enemies = new List<Unit>();

	public static void Create (int _id)
	{
		GameObject unitContainer = (GameObject)GameObject.Instantiate (UnitContainer);
		Unit unit = unitContainer.GetComponent<Unit> ();

		unit.target = unit.source = MapManager.GetRandomPlace ();
		MapManager.UpdateCellMask (unit.source, 1, -1);

		unit.id = _id;

		unit.GetComponent<Transform> ().position = MapManager.GetWorldCoordinates (unit.source);
		
		unit.model = GameObject.Instantiate (UnitTypes[unit.id].model);
		unit.model.GetComponent<Transform> ().SetParent (unit.GetComponent<Transform> ());
		unit.model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0.5f, 0f);
		unit.material = unit.model.GetComponent<Renderer>().material;

		unit.normalColor = unit.model.GetComponent<Renderer> ().material.color;
		unit.speed = UnitTypes[unit.id].speed;

		if (unit.id == 0) {
			Players.Add(unit);
			unit.damage = 5f;
		}
		
		if (unit.id == 1)
			Enemies.Add(unit);
	}

	public static void Idle (Unit _unit) {
		if (_unit.id == 0)
			PlayerAction (_unit);
		
		if (_unit.id == 1)
			EnemyAction (_unit);
	}

	static void EnemyAction(Unit _unit) {
		if (_unit.victim != null) {
			if (_unit.victim.dead) {
				_unit.victim = null;
				return;
			}
			if (MapManager.GetDistance(_unit.source, _unit.victim.source) < 20) {
				Hurt(_unit);
				return;
			}

			if (_unit.timerLock) {
				MakeStep(_unit);
			} else {
				GoTo (_unit, _unit.victim.source);
			}
		} else {
			UpdateTarget(_unit);
		}
	}
	
	static void UpdateTarget(Unit _unit) {
		foreach (var player in Players) {
			int tmpDistance = MapManager.GetDistance (_unit.source, player.source);
			// TODO if distance < attack → break
			
			if (tmpDistance < 200) {
				_unit.DistanceToTarget = tmpDistance;
				Attack(_unit, player);
			}
		}
	}

	static void PlayerAction(Unit _unit) {
		MakeStep(_unit);
	}

	public static void MakeStep(Unit _unit) {
		if (_unit.path == null) {
			if (_unit.target == _unit.source) {
				// Our journey is finished let's get a new journey
				_unit.pathDebug = new List<Cell> ();
//				GoTo (_unit, MapManager.GetRandomPlace ());
			} else {
				// No way to our target, try to find new way
				GoTo (_unit, _unit.target);
			}
			
			return;
		}
		
		if (_unit.path.Count < 2) {
			_unit.path = null;
			return;
		}

		if (_unit.path [1].DirectionLayers [1] == -1) {
			// The next cell is unavailable
			// TODO AI: after pause try again same path
			// FIXME The unit skip cell, when the path was changed
			
			if (!_unit.timerLock)
				_unit.path = null;
			
			return;
		}
		
		MapManager.UpdateCellMask (_unit.path [0], 1, 255);
		_unit.source = _unit.path [1];
		MapManager.UpdateCellMask (_unit.source, 1, -1);
		_unit.path.RemoveAt (0);
	}


	public static void GoTo (Unit _unit, Cell _target)
	{
		_unit.target = _target;
		
		if (!_unit.timerLock) {
			_unit.timerLock = true;
			_unit.TimerLock();
			
			if (_target != _unit.source) {
				_unit.path = MapManager.FindPath (_unit.source, _unit.target);
			}
			
			if (_unit.path != null) {
				// Debug
				_unit.pathDebug = new List<Cell> (_unit.path);
				
				// Game
				_unit.target = _unit.path.Last();
				MakeStep(_unit);
			} else {
				// Debug
				_unit.pathDebug = new List<Cell> ();
				_unit.pathDebug.Add (_unit.source);
				_unit.pathDebug.Add (_unit.target);
				
				// Game
				GoTo (_unit, _unit.target);
			}
		}
	}

	public static void WalkAnimation(Unit _unit)
	{
		Transform trans = _unit.gameObject.GetComponent<Transform> ();
		trans.position = Vector3.MoveTowards (
			trans.position,
			MapManager.GetWorldCoordinates (_unit.source),
			Time.deltaTime * _unit.speed
			);
	}

	static void Highlite (Unit _unit)
	{
		_unit.model.GetComponent<Renderer> ().material.color = _unit.highliteColor;
	}
	
	static void Deselect (Unit _unit)
	{
		_unit.model.GetComponent<Renderer> ().material.color = _unit.normalColor;
	}

	public static void Click (Cell _target)
	{
		GoTo (Players[0], _target);
	}

	public static void Kill(Unit _unit) {
		if (!_unit.dead) {
			_unit.dead = true;
			MapManager.UpdateCellMask (_unit.source, 1, 255);

			// TODO Make array with lists by Unit.Type order
			if (_unit.id == 0) {
				Players.Remove(_unit);
			}
			
			if (_unit.id == 1) {
				Enemies.Remove(_unit);
			}
			
			Create(_unit.id);

			GameObject.Destroy(_unit.gameObject);
		}
	}

	public static void Attack (Unit _assault, Unit _victim) {
		_assault.victim = _victim;
		GoTo(_assault, _victim.source);
	}

	static void Hurt (Unit _assault) {
		if (_assault.victim.health <= 0) {
			Kill (_assault.victim);
			return;
		}
		_assault.victim.health -= _assault.damage;
		_assault.victim.model.GetComponent<Renderer>().material.color = _assault.victim.highliteColor;
	}
}
