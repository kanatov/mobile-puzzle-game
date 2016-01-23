using UnityEngine;

public class Rotate : MonoBehaviour {
	public Vector3 target;
	Transform trans;
	Vector3 targetDir;

	void Awake() {
		trans = this.GetComponent<Transform> ();
	}

	void Update () {
		targetDir = target - trans.position;
		targetDir = new Vector3 (targetDir.x, 0f, targetDir.z);

		Vector3 newDir = Vector3.RotateTowards(trans.forward, targetDir, Time.deltaTime * 7f, 0.0F);
		trans.rotation = Quaternion.LookRotation(newDir);
	}
}
