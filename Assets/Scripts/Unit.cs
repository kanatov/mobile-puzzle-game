using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	// Type of the Unit
	public int id;

	// Look
	public GameObject model;
	public Color normalColor;
	public Material material;

	// Movement
	public Cell source;
	public Cell target;
	public Cell newTarget;
	public float speed;
	public List<Cell> path;
	public List<Cell> pathVis;
	public bool pathLock;
	public float pathLockTime;

	// Attack
	public Unit victim;
	public Unit victimClick;
	public int attackDistance;
	public int viewDistance;
	public float damage;
	public float damageLockTime;
	public bool damageLock;

	// Health
	public float health;
	public float maxHealth;
	public bool dead;
	public GameObject healthPanel;
	public Slider healthSlider;
	float healthPanelOffset = 2.35f;


	void Update () {
		if (GetComponent<Transform> ().position == MapManager.GetWorldCoordinates (source)) {
			UnitManager.Idle(this);
		} else {
			UnitManager.WalkAnimation (this);
			MapManager.DebugDrawPath(pathVis);
		}

		Fade();
		HealthBarPosition();
	}

	void HealthBarPosition () {
		if (healthPanel != null) {
			float x = this.GetComponent<Transform>().position.x;
			float y = this.GetComponent<Transform>().position.y;
			float z = this.GetComponent<Transform>().position.z;
			
			Vector3 worldPos = new Vector3(x,y + healthPanelOffset, z);
			Vector3 screenPos = UnityEngine.Camera.main.WorldToScreenPoint(worldPos);
			healthPanel.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
		}
	}

	void OnMouseUp () {
		UnitManager.UnitClick (this);
	}

	public void PathLock () {
		pathLock = true;
		Invoke ("PathUnlock", pathLockTime);
	}

	void PathUnlock () {
		pathLock = false;
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
		material.color = _color;
	}
}
