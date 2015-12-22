using UnityEngine;

public class Rotate : MonoBehaviour {
	public Vector3 target;
	public Transform transform;
	Vector3 targetDir;

	void Awake() {
		transform = this.GetComponent<Transform> ();
	}

	void Update () {
		targetDir = target - transform.position;
		targetDir = new Vector3 (targetDir.x, 0f, targetDir.z);

		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * 7f, 0.0F);
		transform.rotation = Quaternion.LookRotation(newDir);
	}

}
