using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		InputManager.DetectSwipe();
	}
}
