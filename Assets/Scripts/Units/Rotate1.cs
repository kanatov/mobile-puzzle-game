using UnityEngine;

public class Rotate1 : MonoBehaviour {
	Vector3 target;
	public SwipeDirection Target {
		set {
			int directionsCount = System.Enum.GetValues (typeof(Direction)).Length;
			target = this.GetComponent<Transform> ().eulerAngles;
			float yFloor = Mathf.Floor (target.y / 60) * 60;

			if (value == SwipeDirection.Right) {
				target.y = yFloor + 60;
				if (target.y >= 360) {
					target.y = 359.9f;
				}
			}
			if (value == SwipeDirection.Left) {
				target.y = yFloor - 60;
				if (target.y < 0) {
					transform.eulerAngles = new Vector3 (0, 359.9f, 0);
					target.y = 300;
				}
			}
		}
	}

	Transform transform;

	void Awake() {
		transform = this.GetComponent<Transform> ();
	}

	void Update () {
		if (transform.eulerAngles.y == target.y) {
			if (transform.eulerAngles.y == 359.9f) {
				transform.eulerAngles = Vector3.zero;
			}
			this.enabled = false;
		} else {
			transform.eulerAngles = Vector3.MoveTowards (
				transform.eulerAngles,
				target,
				400f * Time.deltaTime
			);
		}
	}
}
