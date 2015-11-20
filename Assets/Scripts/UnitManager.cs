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
	public static List<Unit> Players = new List<Unit>();
	public static List<Unit> Enemies = new List<Unit>();
	public static UnitType[] UnitTypes;
	public static GameObject UnitContainer;

	// Private
	static Color highliteColor = Color.red;


	public static void Create (int _id) {
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
			unit.damage = 2f;
			unit.attackDistance = 20;
			unit.viewDistance = 50;
			unit.lockTime = 0.5f;
		}
		
		if (unit.id == 1)
			Enemies.Add(unit);
	}


	// Check for the path and make next turn
	public static void Idle (Unit _unit) {
		if (!_unit.damageLock) {
			_unit.DamageLock();
			FindVictim(_unit);
		}


		if (_unit.path == null) {
			if (_unit.target == _unit.source) {
				// Our journey is finished let's get a new journey
				_unit.pathDebug = null;
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
			
			if (!_unit.timerLock)
				_unit.path = null;
			
			return;
		}
		
		MapManager.UpdateCellMask (_unit.path [0], 1, 255);
		_unit.source = _unit.path [1];
		MapManager.UpdateCellMask (_unit.source, 1, -1);
		_unit.path.RemoveAt (0);
	}

	
	static void FindVictim (Unit _unit) {
		List<Unit> units;
		
		if (_unit.id == 0) {
			units = Enemies;
		} else {
			units = Players;
		}
		
		_unit.victim = null;
		int newDistance = 0;
		Unit newVictim = null;

		foreach (var unit in units) {
			int tmpDistance = MapManager.GetDistance (_unit.source, unit.source);
			
			if (tmpDistance < _unit.viewDistance) {
				if (tmpDistance < newDistance || newDistance == 0) {
					newDistance = tmpDistance;
					newVictim = unit;
				}
			}
		}

		if (newVictim != null) {
			Attack (_unit, newVictim);
		}

		AttackVictim (_unit);
	}


	static void AttackVictim (Unit _unit) {
		if (_unit.victim != null) {
			if (_unit.victim.dead) {
				_unit.victim = null;
				return;
			}
			if (MapManager.GetDistance(_unit.source, _unit.victim.source) < _unit.attackDistance) {
				Hurt(_unit);
				return;
			}
		}
	}


	public static void GoTo (Unit _unit, Cell _target) {
		_unit.target = _target;
		
		if (!_unit.timerLock) {
			_unit.TimerLock();
			
			if (_target != _unit.source) {
				_unit.path = MapManager.FindPath (_unit.source, _unit.target);
			}
			
			if (_unit.path != null) {
				// Debug
				_unit.pathDebug = new List<Cell> (_unit.path);
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


	public static void WalkAnimation(Unit _unit) {
		Transform trans = _unit.gameObject.GetComponent<Transform> ();
		trans.position = Vector3.MoveTowards (
			trans.position,
			MapManager.GetWorldCoordinates (_unit.source),
			Time.deltaTime * _unit.speed
			);
	}


	public static void Click (Cell _target) {
		GoTo (Players[0], _target);
	}


	public static void Kill(Unit _unit) {
		// TODO when unit dead the all enemies should forget the path

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


	// TODO minimal distance from where we can attack the unit
	public static void Attack (Unit _assault, Unit _victim) {
		_victim.Highlite(new Color (1f, 0.5f, 0f));
		if (_assault.victim == null || _assault.victim != _victim) {
			_assault.victim = _victim;
			GoTo(_assault, _victim.source);
		}
	}


	static void Hurt (Unit _assault) {
		if (_assault.victim.health <= 0) {
			_assault.path = null;
			Kill (_assault.victim);
			return;
		}
		_assault.victim.health -= _assault.damage;
		_assault.victim.Highlite(highliteColor);
	}
}
