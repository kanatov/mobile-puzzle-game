using UnityEngine;
using System.Collections;

public class WaypointCollider : MonoBehaviour {
	public Waypoint waypoint;
	
	void OnMouseUp() {
		DynamicObject.player.GoTo(waypoint);
	}
}
