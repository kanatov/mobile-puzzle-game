using UnityEngine;
using System.Collections;

public class Highlite : MonoBehaviour {
	public Cell[] neighbours;

	void OnMouseEnter() {
		Debug.Log ("Enter: " + neighbours.Length);
		foreach (var n in neighbours) {
			if (n == null) {
				continue;
			}
			n.terrainModel.GetComponent<Renderer>().material.color = Color.red;
		}

	}
	void OnMouseExit() {
		foreach (var n in neighbours) {
			if (n == null) {
				continue;
			}
			n.terrainModel.GetComponent<Renderer>().material.color = Color.white;
		}
	}

}
