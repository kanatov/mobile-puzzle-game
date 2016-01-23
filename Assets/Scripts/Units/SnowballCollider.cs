using UnityEngine;
using System.Collections;

public class SnowballCollider : MonoBehaviour {
	public Snowball snowball;
	bool mouseDown = false;

	void OnMouseDown() {
		mouseDown = true;
	}

	void OnMouseUp() {
		mouseDown = false;
		InputController.DetectSwipe (snowball);
	}

	void Update() {
		if (mouseDown) {
			InputController.DetectSwipe (snowball);
		}
	}
}
