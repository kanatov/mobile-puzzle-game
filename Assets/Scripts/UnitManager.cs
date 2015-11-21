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
	public static Canvas canvas;

	// Private
	static Color highliteColor = new Color(1f, 0.5f, 0f);

	public static void Create (int _id) {
		GameObject unitContainer = (GameObject)GameObject.Instantiate (UnitContainer);
		Unit unit = unitContainer.GetComponent<Unit> ();

		// Type of the Unit
		unit.id = _id;

		// Position
		unit.source = MapManager.GetRandomPlace ();
		unit.target = unit.source;
		MapManager.UpdateCellMask (unit.source, 1, -1);
		unit.GetComponent<Transform> ().position = MapManager.GetWorldCoordinates (unit.source);

		// Look
		unit.model = GameObject.Instantiate (UnitTypes[unit.id].model);
		unit.model.GetComponent<Transform> ().SetParent (unit.GetComponent<Transform> ());
		unit.model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0.5f, 0f);

		unit.material = unit.model.GetComponent<Renderer>().material;
		unit.normalColor = unit.material.color;
		
		// Movement
		unit.speed = UnitTypes[unit.id].speed;
		unit.pathLock = false;
		unit.pathLockTime = 1f;
		
		// Attack
		unit.attackDistance = UnitTypes[unit.id].attackDistance;
		unit.viewDistance = UnitTypes[unit.id].viewDistance;
		unit.damage = UnitTypes[unit.id].damage;
		unit.damageLockTime = UnitTypes[unit.id].damageLockTime;
		unit.damageLock = false;

		// Health
		unit.maxHealth = UnitTypes[unit.id].maxHealth;
		unit.health = unit.maxHealth;
		unit.dead = false;
		unit.healthPanel = (GameObject) GameObject.Instantiate(UI[0]);
		unit.healthPanel.GetComponent<Transform>().SetParent(canvas.GetComponent<Transform>(), false);
		unit.healthSlider = unit.healthPanel.GetComponentInChildren<Slider>();
		unit.healthPanel.SetActive(false);

		if (unit.id == 0) {
			Players.Add(unit);
			unit.healthPanel.SetActive(true);
		} else {
			Enemies.Add(unit);
		}
	}


	// Check for the path and make next turn
	public static void Idle (Unit _unit) {
		if (!_unit.damageLock && _unit.victimClick == null) {
			// Find victim
			_unit.victim = null;

			List<Unit> victims;

			if (_unit.id == 0) {
				victims = Enemies;
			} else {
				victims = Players;
			}
			
			int newDistance = 0;
			
			foreach (var victim in victims) {
				int tmpDistance = MapManager.GetDistance (_unit.source, victim.source);
				
				if (tmpDistance <= _unit.viewDistance) {
					if (tmpDistance < newDistance || newDistance == 0) {
						newDistance = tmpDistance;
						_unit.victim = victim;
					}
				}
			}
		}

		if (_unit.victim == null) {
			// Update target
			if (_unit.newTarget != null && !_unit.pathLock) {
				_unit.target = _unit.newTarget;
				_unit.newTarget = null;
			}
			
			// Back to source position after attack
			if (_unit.target == _unit.source) {
				_unit.path = null;
				_unit.pathVis = null;
				return;
			}

			GoTo (_unit, _unit.target);
		} else {
			if (MapManager.GetDistance(_unit.source, _unit.victim.source) <= _unit.attackDistance) {
				Hurt(_unit);
				_unit.path = null;
				return;
			}
			GoTo (_unit, _unit.victim.source);
		}

		// We need this check becouse of timer locks
		if (_unit.path == null) {
			return;
		}

		// Make Step
		if (_unit.path.Count == 1) {
			Debug.Log ("_unit.path.Count = " + _unit.path.Count);
			return;
		}

		if (_unit.path [1].DirectionLayers [1] == -1) {
			return;
		}
		
		MapManager.UpdateCellMask (_unit.path [0], 1, 255);
		_unit.source = _unit.path [1];
		MapManager.UpdateCellMask (_unit.source, 1, -1);
		_unit.path.RemoveAt (0);
	}


	static void GoTo (Unit _unit, Cell _target) {
		if (_unit.pathLock) {
			return;
		}
		_unit.PathLock();

		if (_target == _unit.source) {
			return;
		}
			
		_unit.path = MapManager.FindPath (_unit.source, _target);
		
		if (_unit.path != null) {
			// Debug
			_unit.pathVis = new List<Cell> (_unit.path);
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
		Players[0].newTarget = _target;
	}


	public static void UnitClick (Unit _unit) {
		if (!Players.Contains(_unit)) {
			_unit.Highlite(new Color (0f, 1f, 1f));

			Players[0].victimClick = _unit;
			Players[0].victim = _unit;
		}
	}


	static void Hurt (Unit _assault) {
		if (_assault.damageLock) {
			return;
		}
		_assault.DamageLock();

		_assault.victim.healthPanel.SetActive(true);
		_assault.victim.health -= _assault.damage;
		_assault.victim.healthSlider.value = _assault.victim.health/_assault.victim.maxHealth;		
		_assault.victim.Highlite(highliteColor);
		
		if (_assault.victim.health <= 0 && !_assault.victim.dead) {
			_assault.victim.dead = true;

			MapManager.UpdateCellMask (_assault.victim.source, 1, 255);
			
			if (_assault.victim.id == 0) {
				Players.Remove(_assault.victim);
			}
			
			if (_assault.victim.id == 1) {
				Enemies.Remove(_assault.victim);
			}
			
			Create(_assault.victim.id);
			
			GameObject.Destroy(_assault.victim.healthPanel);
			GameObject.Destroy(_assault.victim.gameObject);
			
			if (_assault.victimClick == _assault.victim) {
				_assault.victimClick = null;
				_assault.target = _assault.source;
			}

			_assault.victim = null;
		}
	}
}

