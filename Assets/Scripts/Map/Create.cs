using UnityEngine;
using System.Collections;

public class Create : MonoBehaviour {
	Transform trans;
	Vector3 source = new Vector3 (0.05f, 0.05f, 0.05f);
	Vector3 target = new Vector3 (1f, 1f, 1f);

	void OnEnable () {
		trans = this.GetComponent<Transform>();
		trans.localScale = source;
	}

	void Update () {
		if (trans.localScale != target) {
			trans.localScale = Vector3.MoveTowards (
				trans.localScale,
				target,
				Time.deltaTime * 10
				);
		} else {
			this.enabled = false;
		}
	}
}
