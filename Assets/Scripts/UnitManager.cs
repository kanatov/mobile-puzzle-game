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
		unit.pathLockTime = 0.5f;
		
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
	public static void Behaviour (Unit _unit) {

		// If Damgae not locked
		// If we have the victimBoss
		// If the victimBoss is acceptable
		// Hurt victim boss
		// else
		// if other targets acceptable
		// hurt other targets

		if (!_unit.damageLock) {
			_unit.DamageLock();

			if (_unit.victimBoss == null) {
				_unit.victim = null;

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
					
					if (tmpDistance <= _unit.viewDistance) {
						if (tmpDistance < newDistance || newDistance == 0) {
							newDistance = tmpDistance;
							_unit.victim = victim;
						}
					}
				}
			}

			if (_unit.victim != null) {
				if (MapManager.GetDistance(_unit.source, _unit.victim.source) <= _unit.attackDistance) {
					Hurt(_unit);
					_unit.target = _unit.source;
					return;
				}

				_unit.target = _unit.victim.source;
			}
		}

		GoTo (_unit, _unit.target);
		Walk (_unit);
	}

	static void Walk(Unit _unit) {
		if (_unit.GetComponent<Transform> ().position != MapManager.GetWorldCoordinates (_unit.source)) { 
			Transform trans = _unit.gameObject.GetComponent<Transform> ();
			trans.position = Vector3.MoveTowards (
				trans.position,
				MapManager.GetWorldCoordinates (_unit.source),
				Time.deltaTime * _unit.speed
				);
		} else {
			// We need this check becouse of timer locks
			if (_unit.path == null) {
				return;
			}
			
			if (_unit.path.Count == 1) {
				_unit.path = null;
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
	}

	static void GoTo (Unit _unit, Cell _target) {
		if (_unit.source == _target) {
			_unit.pathVis = null;
			_unit.path = null;
			return;
		}

		if (_unit.pathLock) {
			return;
		}
		_unit.PathLock();

		_unit.path = MapManager.FindPath (_unit.source, _target);
		
		// Debug
		if (_unit.path != null) {
			_unit.pathVis = new List<Cell> (_unit.path);
		}
	}


	public static void Click (Cell _target) {
		Players[0].path = null;
		Players[0].target = _target;
		Players[0].victim = null;
		Players[0].pathLock = false;
		GoTo (Players[0], Players[0].target);
	}


	public static void UnitClick (Unit _unit) {
		if (!Players.Contains(_unit)) {
			_unit.Highlite(new Color (0f, 1f, 1f));

			Players[0].victimBoss = _unit;
			Players[0].victim = _unit;
			Players[0].target = Players[0].victim.source;
			Players[0].pathLock = false;
			GoTo (Players[0], Players[0].target);
		}
	}


	static void Hurt (Unit _assault) {
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
			
			if (_assault.victimBoss == _assault.victim) {
				_assault.victimBoss = null;
				_assault.target = _assault.source;
			}

			_assault.victim = null;
		}
	}
}

