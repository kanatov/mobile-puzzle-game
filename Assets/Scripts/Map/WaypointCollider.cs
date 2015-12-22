using UnityEngine;
using System.Collections;

public class WaypointCollider : MonoBehaviour {
	public Waypoint waypoint;
	
	void OnMouseUp() {
		UnitBehaviour.GoTo (UnitBehaviour.player, waypoint);
	}
}
