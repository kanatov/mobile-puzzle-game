using UnityEngine;
using System.Collections;

public class WaypointCollider : MonoBehaviour {
	public Waypoint waypoint;
	
	void OnMouseUp() {
		waypoint.Click ();
	}
}
