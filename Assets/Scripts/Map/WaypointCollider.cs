using UnityEngine;
using System.Collections;

public class WaypointCollider : MonoBehaviour {
	public int waypoint;
	
	void OnMouseUp() {
		MapController.player.GoTo (MapController.waypoints [waypoint]);
	}
}
