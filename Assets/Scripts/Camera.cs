using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {
	public GameObject camera;

	public void SetPosition () {
		float z = Overview.overviewMasks[Player.overview].GetLength(1)/2;
		this.GetComponent<Transform>().position = new Vector3(0f, 0f, z);
	}
}
