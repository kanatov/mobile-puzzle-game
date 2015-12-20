using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move : MonoBehaviour {
	public List<Waypoint> path;
	public Waypoint source;
	
	// Update is called once per frame
	void Update () {
		if (this.GetComponent<Transform> ().position == source.position) {
			if (path != null && path.Count > 1) {
				this.GetComponent<Rotate> ().target = path[1];
				this.GetComponent<Rotate> ().enabled = true;

				foreach (var _trigger in path [1].triggers) {
					_trigger.Launch ();
				}

				source = path[1];
				path.RemoveAt (0);

			} else {
				this.enabled = false;
				this.GetComponent<Rotate> ().enabled = false;
			}
		} else {
			GetComponent<Transform> ().position = Vector3.MoveTowards (
				GetComponent<Transform> ().position,
				source.position,
				Time.deltaTime * 2f
			);
		}
	}
}
