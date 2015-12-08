using UnityEngine;

[System.Serializable]
public class Unit : MonoBehaviour {

	// Type of the Unit
	public int id;

	// Look
	public GameObject model;
	public Color normalColor;
	public Material material;

	// Movement
	public float speed;
	public int overview;

	// Attack
	public int attackDistance;
	public int viewDistance;
	public float damage;

	// Health
	public bool dead;
	public float maxHealth;
	public float health;

}
