using UnityEngine;
using System.Collections;

public class WaypointCollider : MonoBehaviour {
	public Waypoint waypoint;
	
	void OnMouseUp() {
		if (waypoint.activateOnTouch) {
			waypoint.ActivateTriggers ();
		} else {
			MapController.player.GoTo (MapController.waypoints [waypoint.id]);
		}
	}
}
