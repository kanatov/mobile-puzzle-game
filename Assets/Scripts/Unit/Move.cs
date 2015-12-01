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
//		if (unit.source == null) {
			target = Map.GetZeroPosition();
			unit.speed = Map.hexSpeed;

//		} else {
//			target = Map.GetWorldCoordinates (unit.source.x, unit.source.y);
//		}

	}

	void Update () {
		if (trans.position != Map.GetZeroPosition()) {
			trans.position = Vector3.MoveTowards (
				trans.position,
				Map.GetZeroPosition(),
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
