using UnityEngine;

public class Move : MonoBehaviour {
	public Vector3 target;
	public Transform transform;
	public Unit unit;

	void Awake() {
		transform = this.GetComponent<Transform> ();
	}

	void Update () {
		if (transform.position == target) {
			this.enabled = false;

			if (this.GetComponent<Rotate> () != null) {
				this.GetComponent<Rotate> ().enabled = false;
				UnitBehaviour.MakeStep (unit);
			}
		}

		transform.position = Vector3.MoveTowards (
			transform.position,
			target,
			Time.deltaTime * 2f
		);
	}
}
