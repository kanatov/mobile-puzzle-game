using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	Transform trans;
	Unit unit;
	Vector3 target;

	void OnEnable () {
		trans = this.GetComponent<Transform>();
		unit = this.GetComponent<Unit>();
		GameController.turnLockQueue++;
		target = Map.GetMapContainerPosition();
		unit.speed = Map.hexSpeed;
	}

	void Update () {
		if (trans.position != Map.GetMapContainerPosition()) {
			trans.position = Vector3.MoveTowards (
				trans.position,
				Map.GetMapContainerPosition(),
				Time.deltaTime * unit.speed
				);
		} else {
			if (unit.source == null) {
				GameController.turnLockQueue--;
			}
			this.enabled = false;
		}
	}
}
