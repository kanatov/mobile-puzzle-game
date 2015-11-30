using UnityEngine;
using System.Collections;

public class Remove : MonoBehaviour {
	Transform trans;
	Vector3 target = new Vector3 (0.05f, 0.05f, 0.05f);

	void OnEnable () {
		trans = this.GetComponent<Transform>();
	}

	void Update () {
		if (trans.localScale != target) {
			trans.localScale = Vector3.MoveTowards (
				trans.localScale,
				target,
				Time.deltaTime * 2
				);
		} else {
			this.enabled = false;
			GameObject.Destroy (this.gameObject);
		}
	}
}
