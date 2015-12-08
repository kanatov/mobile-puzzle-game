using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	Transform trans;
	Vector3 target;
	float speed;

//	void OnEnable () {
//		GameController.AnimationsCounter = 1;
//		trans = this.GetComponent<Transform>();
//
//		if (this.GetComponent<Unit>() == null) {
//			target = Map.GetPlayerWorldCoordinates();
//			speed = Map.hexSpeed;
//		} else {
//			Unit unit = this.GetComponent<Unit>();
//			speed = Map.hexSpeed;
//			target = new Vector3(0f, 0f, 0f);
//		}
//	}

//	void Update () {
//		Animations.Move(this, trans, target, speed);
//	}
}
