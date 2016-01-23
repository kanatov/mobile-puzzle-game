using UnityEngine;
using System.Collections;

public class NodeCollider : MonoBehaviour {
	public Node node;
	
	void OnMouseUp() {
		if (node.activateOnTouch) {
			node.ActivateTriggers ();
		} else {
			MapController.player.GoTo (MapController.walkNodes [node.id]);
		}
	}
}
