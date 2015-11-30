using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	Transform trans;
	Unit unit;
	Vector3 target;

	void OnEnable () {
		trans = this.GetComponent<Transform>();
		unit = this.GetComponent<Unit>();

		if (unit.source == null) {
			target = new Vector3 (-Player.x, 0, Player.y);
			unit.speed = 2;
		} else {
			target = Map.GetWorldCoordinates (unit.source);
		}

	}

	void Update () {
		if (trans.position != target) {
			trans.position = Vector3.MoveTowards (
				trans.position,
				target,
				Time.deltaTime * unit.speed
				);
		} else {
			this.enabled = false;
		}
	}
}
