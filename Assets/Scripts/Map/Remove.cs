using UnityEngine;
using System.Collections;

public class Remove : MonoBehaviour {
	Transform trans;

	void OnEnable () {
		trans = this.GetComponent<Transform>();
		GameController.turnLockQueue++;
	}

	void Update () {
		if (trans.localScale != Map.hexSmallScale) {
			trans.localScale = Vector3.MoveTowards (
				trans.localScale,
				Map.hexSmallScale,
				Time.deltaTime * Map.hexSpeed
				);
		} else {
			GameController.turnLockQueue--;
			this.enabled = false;
			GameObject.Destroy (this.gameObject);
		}
	}
}
