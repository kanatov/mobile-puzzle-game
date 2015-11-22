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
		unit.pathLockTime = 0.3f;
		
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
	public static void Attack (Unit _unit) {
		if (!_unit.damageLock) {
			// We use damage lock here, becouse of the foreach
			_unit.DamageLock();

			if (_unit.victimFollow != null && MapManager.GetDistance (_unit.source, _unit.victimFollow.source) <= _unit.attackDistance ) {
				Hurt (_unit, _unit.victimFollow);
			} else {
				Unit newVictim = null;
				int currentDistance = _unit.viewDistance + 1;
				
				// Find victim
				List<Unit> victims;
				
				if (_unit.id == 0) {
					victims = Enemies;
				} else {
					victims = Players;
				}
				
				foreach (var victim in victims) {
					int newDistance = MapManager.GetDistance (_unit.source, victim.source);
					
					if (newDistance <= _unit.viewDistance && newDistance < currentDistance) {
						currentDistance = newDistance;
						newVictim = victim;
					}
				}

				if (newVictim != null) {
					if (currentDistance <= _unit.attackDistance) {
						Hurt (_unit, newVictim);
					} else {
						if (_unit.cellFollow == null && _unit.victimFollow == null) {
							_unit.victimFollow = newVictim;
						}
					}
				}
			}
		}
	}

	public static void Walk(Unit _unit) {
		if (_unit.GetComponent<Transform> ().position == MapManager.GetWorldCoordinates (_unit.source)) {
			if (_unit.source == _unit.cellFollow) {
				_unit.path = null;
				_unit.cellFollow = null;
				return;
			}

			if (_unit.cellFollow == null && _unit.victimFollow == null) {
				_unit.path = null;
				return;
			}

			if (!_unit.pathLock) {
				_unit.PathLock();

				if (_unit.cellFollow != null) {
					_unit.path = MapManager.FindPath (_unit.source, _unit.cellFollow);
				}

				if (_unit.victimFollow != null) {
					_unit.path = MapManager.FindPath (_unit.source, _unit.victimFollow.source);
				}
			}

			// We need this check becouse of timer locks
			if (_unit.path == null) {
				return;
			}
			
			if (_unit.path.Count == 1) {
				Debug.LogWarning ("Path.Count == 1");
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

		Transform trans = _unit.gameObject.GetComponent<Transform> ();
		trans.position = Vector3.MoveTowards (
			trans.position,
			MapManager.GetWorldCoordinates (_unit.source),
			Time.deltaTime * _unit.speed
			);
	}


	public static void SetCellTarget (Cell _target) {
		Players[0].cellFollow = _target;
		Players[0].victimFollow = null;
		Players[0].pathLock = false;
	}


	public static void SetUnitTarget (Unit _unit) {
		_unit.Highlite(new Color (0f, 1f, 1f));

		Players[0].victimFollow = _unit;
		Players[0].cellFollow = null;
		Players[0].pathLock = false;
	}


	static void Hurt (Unit _assault, Unit _victim) {
		if (_victim == null) {
			return;
		}

		_victim.healthPanel.SetActive(true);
		_victim.health -= _assault.damage;
		_victim.healthSlider.value = _victim.health/_victim.maxHealth;		
		_victim.Highlite(highliteColor);

		if (_victim.health <= 0 && !_victim.dead) {
			_victim.dead = true;

			MapManager.UpdateCellMask (_victim.source, 1, 255);
			
			if (_victim.id == 0) {
				Players.Remove(_victim);
			}
			
			if (_victim.id == 1) {
				Enemies.Remove(_victim);
			}
			
			Create(_victim.id);
			
			GameObject.Destroy(_victim.healthPanel);
			GameObject.Destroy(_victim.gameObject);
			
			if (_assault.victimFollow == _victim) {
				_assault.victimFollow = null;
			}
		}
	}
}

