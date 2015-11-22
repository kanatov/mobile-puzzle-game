using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitType {

	public GameObject model;
	public float speed;
	public float maxHealth;
	public float damage;
	public float damageLockTime;
	public int viewDistance;
	public int attackDistance;
	public int directionLayer;
}
