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
	public Transform transform;
	public DynamicObject dynamicObject;

	void Awake() {
		transform = this.GetComponent<Transform> ();
	}

	void Update () {
		if (transform.position == Path[0]) {
			if (Path.Count == 1) {
				this.enabled = false;
				dynamicObject.Move ();

				return;
			}

			Path.RemoveAt (0);
		}

		transform.position = Vector3.MoveTowards (
			transform.position,
			Path[0],
			Time.deltaTime * 2f
		);
	}
}
