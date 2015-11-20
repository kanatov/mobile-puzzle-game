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
// TODO Aray of assaulter of victim
// TODO Attack if possible by distance 
// TO DO Movement out of grid

public static class UnitManager {

	// Public
	public static List<Unit> Players = new List<Unit>();
	public static List<Unit> Enemies = new List<Unit>();
	public static UnitType[] UnitTypes;
	public static GameObject UnitContainer;

	// Private
	static Color highliteColor = new Color(1f, 0.5f, 0f);


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
			unit.damage = 3f;
			unit.attackDistance = 20;
			unit.viewDistance = 50;
			unit.lockTime = 0.3f;
		}
		
		if (unit.id == 1)
			Enemies.Add(unit);
	}


	// Check for the path and make next turn
	public static void Idle (Unit _unit) {
		if (!_unit.damageLock) {
			// Find victim

			List<Unit> victims;
			
			if (_unit.id == 0) {
				victims = Enemies;
			} else {
				victims = Players;
			}
			
			int newDistance = 0;
			
			foreach (var victim in victims) {
				int tmpDistance = MapManager.GetDistance (_unit.source, victim.source);
				
				if (tmpDistance < _unit.viewDistance) {
					if (tmpDistance < newDistance || newDistance == 0) {
						newDistance = tmpDistance;
						_unit.victim = victim;
					}
				}
			}
		}

		if (_unit.victim == null) {
			// Follow the path
			if (_unit.target == _unit.source) {
				return;
			}

			if (_unit.path != null) {
				MakeStep(_unit);
			} else {
				_unit.pathDebug = null;
				if (_unit.unitClick) {
					_unit.unitClick = false;
					_unit.target = _unit.source;
				} else {
					GoTo (_unit, _unit.target);
				}
			}
		} else {
			// Attack or follow
			if (MapManager.GetDistance(_unit.source, _unit.victim.source) <= _unit.attackDistance) {
				Hurt(_unit);
			} else {
				GoTo (_unit, _unit.victim.source);
				MakeStep(_unit);
			}
		}

		_unit.DamageLock();
	}


	static void MakeStep(Unit _unit) {
		if (_unit.path.Count < 2) {
			_unit.path = null;
			return;
		}
		
		if (_unit.path [1].DirectionLayers [1] == -1) {
			return;
		}

		if (!_unit.timerLock) {
			GoTo(_unit, _unit.target);
		}

		MapManager.UpdateCellMask (_unit.path [0], 1, 255);
		_unit.source = _unit.path [1];
		MapManager.UpdateCellMask (_unit.source, 1, -1);
		_unit.path.RemoveAt (0);
	}


	static void GoTo (Unit _unit, Cell _target) {
		if (!_unit.timerLock) {
			_unit.TimerLock();
			
			_unit.path = MapManager.FindPath (_unit.source, _target);
			
			if (_unit.path != null) {
				// Debug
				_unit.pathDebug = new List<Cell> (_unit.path);
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
		Players[0].target = _target;
	}


	public static void UnitClick (Unit _unit) {
		if (!Players.Contains(_unit)) {
			_unit.Highlite(new Color (0f, 1f, 1f));

			Players[0].unitClick = true;
			Players[0].victim = _unit;
		}
	}


	static void Hurt (Unit _assault) {
		if (_assault.damageLock) {
			return;
		}
		_assault.DamageLock();
		
		_assault.victim.health -= _assault.damage;
		_assault.victim.Highlite(highliteColor);
		
		if (_assault.victim.health <= 0 || _assault.victim.dead) {
			Kill (_assault);
		}
	}


	public static void Kill(Unit _unit) {
		if (!_unit.victim.dead) {
			_unit.victim.dead = true;
			MapManager.UpdateCellMask (_unit.victim.source, 1, 255);

			// TODO Make array with lists by Unit.Type order
			if (_unit.victim.id == 0) {
				Players.Remove(_unit.victim);
			}
			
			if (_unit.id == 1) {
				Enemies.Remove(_unit.victim);
			}
			
			Create(_unit.victim.id);

			GameObject.Destroy(_unit.victim.gameObject);

			_unit.path = null;
			_unit.victim = null;
		}
	}
}
