using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public GameObject model;
	public int id;
	public float speed;
	public Cell source;
	public Color highliteColor = Color.red;
	public Color normalColor;
	public bool dead = false;
	public Cell target;
	public Unit victim;
	public List<Cell> path = null;
	public List<Cell> pathDebug = new List<Cell> ();
	public bool timerLock = false;
	public int DistanceToTarget = 0;
	public float health = 10f;
	public float damage = 1f;
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
		Invoke ("TimerUnlock", 1f);
	}

	void TimerUnlock () {
		timerLock = false;
	}

	void Fade() {
		if (material.color != normalColor) {
			material.color = Color.Lerp (material.color, normalColor, 0.1f);
		}
	}
}
