using UnityEngine;
using System.Collections;

public class Add : MonoBehaviour {
	Transform trans;
	Vector3 target = new Vector3 (1f, 1f, 1f);

	void OnEnable () {
		trans = this.GetComponent<Transform>();
		trans.localScale = Map.hexSmallScale;
		GameController.turnLockQueue++;
	}

	void Update () {
		if (trans.localScale != target) {
			trans.localScale = Vector3.MoveTowards (
				trans.localScale,
				target,
				Time.deltaTime * Map.hexSpeed
				);
		} else {
			GameController.turnLockQueue--;
			this.enabled = false;
		}
	}
}
