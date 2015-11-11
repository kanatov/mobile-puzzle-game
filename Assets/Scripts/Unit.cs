using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Unit : MonoBehaviour {

	// Public data
	public Main Main;
	public GameObject prefab;
	public float speed;
	// Direction group depend the allowed direction layer
	// 0. Ground
	public int DirectionGroup;
	public int PlayerGroup;

	// Private data
	Node target;
	Node stepTarget;
	Node source;
	List<Node> path = null;
	List<Node> pathDebug = new List<Node>();
	bool go = true;
	int counter = 0;
	int updatePath = 10;

	// Debug
	Color debugPathColor = Color.yellow;


	void Update() {
		// Debug Path
		int currNode = 0;
		
		while (currNode < pathDebug.Count - 1) {
			Node startNode = pathDebug [currNode];
			Vector3 start = Main.GetWorldCoordinates (startNode);
			
			Node endNode = pathDebug [currNode + 1];
			Vector3 end = Main.GetWorldCoordinates (endNode);
			
			Debug.DrawLine (start, end, debugPathColor);
			
			currNode++;
		}

		// Walk animation
		GetComponent<Transform> ().position = Vector3.MoveTowards(
			GetComponent<Transform> ().position,
			Main.GetWorldCoordinates(stepTarget),
			Time.deltaTime * speed
		);

		if (GetComponent<Transform> ().position == Main.GetWorldCoordinates (stepTarget) && go) {
			// Unit animation completed
			NextStep ();
		}
	}

	void OnMouseUp() {
		Main.Select(this.gameObject);
	}

	public void Select() {
		prefab.GetComponent<Renderer>().material.color = Color.red;
	}

	public void Deselect() {
		prefab.GetComponent<Renderer>().material.color = Color.white;
	}

	public void GoTo(Node _target) {
		if (source != _target && stepTarget != _target) {

			target = _target;
			counter = 0;
			path = Main.Map.FindPath(stepTarget, target);

			if (path != null) {
				// Debug
				pathDebug = new List<Node>(path);
				debugPathColor = Color.yellow;

				// Game
				stepTarget = path[0];
				go = true;
			} else {
				// Debug
				pathDebug = new List<Node>();
				pathDebug.Add(stepTarget);
				pathDebug.Add(target);
				debugPathColor = Color.red;

				// Game
				go = false;

				Invoke("TryGoTo", 1);
			}
		}
	}

	void TryGoTo() {
		GoTo (target);
	}

	void NextStep() {
		source = stepTarget;

		if (path != null) {
			if (path.Count == 1) {
				path = null;
				return;
			}

			if (path[1].DirectionLayers[DirectionGroup] == -1) {
				// The next cell is unavailable
				// TODO relation between units
				// TODO units go through each other if it's only the one way
				path = null;
				return;
			}

			if (target != source) {
				// Unit target is not achived yet
				MakeNextStep();
			} else {
				path = null;
			}
		} else {
			// We have no path
			if (target == source) {
				pathDebug = new List<Node>();
				GoTo (Main.Map.GetRandomPlace());
			} else {
				// The array is finished, but our goal is not achieved yet
				// Probably we should wait some time, or it is not posible
				GoTo (target);
			}
		}
	}

	void MakeNextStep() {
		if (counter < updatePath) {
			counter++;
			Main.Map.UpdateNodeMask (path [0], 0, 255);
		
			stepTarget = path [1];
		
			Main.Map.UpdateNodeMask (stepTarget, 0, -1);
		
			path.RemoveAt (0);
		} else {
			GetRandomUpdate ();
			GoTo (target);
		}
	}

	public void Place(Node _target) {
		// It is important to get a Pos variable from real node
		target = source = stepTarget = _target;
		this.GetComponent<Transform> ().position = Main.GetWorldCoordinates (source);
		Main.Map.UpdateNodeMask (source, 0, -1);

		speed = UnityEngine.Random.Range (1.5f, 2.5f); 
		DirectionGroup = 0; 
		PlayerGroup = 0; 
	}

	void GetRandomUpdate () {
		updatePath = Random.Range (10, 20);
	}
}
