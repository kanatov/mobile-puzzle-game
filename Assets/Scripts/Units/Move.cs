using UnityEngine;
using System.Collections.Generic;

public class Move : MonoBehaviour {
	int currentStep;
	List<Vector3> path;
	public List<Vector3> Path {
		get {
			return path;
		}
		set {
			path = value;
			this.enabled = true;
		}
	}
	public Transform trans;
	public DynamicObject dynamicObject;

	void Awake() {
		trans = this.GetComponent<Transform> ();
	}

	void Update () {
		if (trans.position == Path[0]) {
			if (Path.Count == 1) {
				this.enabled = false;
				dynamicObject.Move ("move");

				return;
			}

			Path.RemoveAt (0);
		}

		trans.position = Vector3.MoveTowards (
			trans.position,
			Path[0],
			Time.deltaTime * 2f
		);
	}
}
