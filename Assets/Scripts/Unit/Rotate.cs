using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	Transform trans;
	Vector3 targetPos;
	float speed;

	void OnEnable () {
//		GameController.AnimationsCounter = 1;
//		trans = this.GetComponent<Transform>();
//
//		if (this.GetComponent<Unit>() == null) {
//			targetPos = Map.GetPlayerWorldCoordinates();
//			speed = Map.hexSpeed;
//		} else {
//			Unit unit = this.GetComponent<Unit>();
//			speed = Map.hexSpeed;
//			targetPos = new Vector3(0f, 0f, 0f);
//		}
	}

	void Update () {
//		Animations.Move(this, trans, targetPos, speed);
	}
}
