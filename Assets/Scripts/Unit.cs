using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public GameObject model;
	public int id;
	public float speed;
	public Cell source;
	public Color normalColor;
	public bool dead = false;
	public Cell target;
	public Unit victim;
	public List<Cell> path = null;
	public List<Cell> pathDebug = new List<Cell> ();
	public bool timerLock = false;
	public int DistanceToTarget = 0;
	public int attackDistance = 20;
	public int viewDistance = 100;
	public float health = 10f;
	public float damage = 1f;
	public float damageLockTime = 0.5f;
	public bool damageLock = false;
	public float lockTime = 1f;
	public Material material;

	void Update () {
		if (GetComponent<Transform> ().position == MapManager.GetWorldCoordinates (source)) {
			UnitManager.Idle(this);
		} else {
			UnitManager.WalkAnimation (this);
			MapManager.DebugDrawPath(pathDebug);
		}

		Fade();
	}

	void OnMouseUp () {
		if (!UnitManager.Players.Contains(this)) {
			UnitManager.Attack (UnitManager.Players[0], this);
		}
	}

	public void TimerLock () {
		timerLock = true;
		Invoke ("TimerUnlock", lockTime);
	}

	void TimerUnlock () {
		timerLock = false;
	}

	public void DamageLock () {
		damageLock = true;
		Invoke ("DamageUnlock", damageLockTime);
	}
	
	void DamageUnlock () {
		damageLock = false;
	}
	
	void Fade() {
		if (material.color != normalColor) {
			material.color = Color.Lerp (material.color, normalColor, 0.5f);
		}
	}

	public void Highlite (Color _color) {
		model.GetComponent<Renderer>().material.color = _color;
	}
}
