using UnityEngine;
using System.Collections.Generic;

public class Size : MonoBehaviour {
	Transform trans;
	Vector3 target;

	void OnEnable () {
		trans = this.GetComponent<Transform>();
		GameController.TurnLock = 1;

		if (this.GetComponent<Cell>().die) {
			target = Map.hexSmallScale;
		} else {
			target = new Vector3(1f, 1f, 1f);
		}
	}

	void Update () {
		Animations.Size (this, trans, target, Map.hexSpeed);
	}
}
