using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		FollowPlayer();
	}

	void FollowPlayer () {
		if (UnitManager.Players.Count != 0) {
			GetComponent<Transform>().position = UnitManager.Players[0].GetComponent<Transform>().position;
		}
	}
}
