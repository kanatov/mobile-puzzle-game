using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {
	public GameObject camera;

	public void SetPosition () {
		float z = Player.overview + 1;
		this.GetComponent<Transform>().position = new Vector3(0f, 0f, z);
	}
}
