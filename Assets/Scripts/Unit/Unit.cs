using UnityEngine;

public class Unit : MonoBehaviour {

	// Type of the Unit
	public int id;

	// Look
	public GameObject model;
	public Color normalColor;
	public Material material;

	// Movement
	public Direction direction = Direction.None;
	public float speed;
	public int directionLayer;

	// Attack
	public int attackDistance;
	public int viewDistance;
	public float damage;

	// Health
	public bool dead;
}
