using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Unit : MonoBehaviour
{

	// Public data
	public Main Main;
	public Map Map;
	// Direction group depend the allowed direction layer
	// 0. Ground
	public int Type;
	public UnitType[] UnitTypes;

	// Private data
	GameObject model;

	Cell target;
	Cell stepTarget;
	Cell source;

	List<Cell> path = null;
	List<Cell> pathDebug = new List<Cell> ();

	bool go = true;
	int counter = 0;
	int updatePath = 10;

	// Debug
	Color debugPathColor = Color.yellow;

	public void Init (int _type, Cell _target)
	{
		Type = _type;
		target = source = stepTarget = _target;
		this.GetComponent<Transform> ().position = Map.GetWorldCoordinates (_target);

		model = (GameObject)Instantiate (UnitTypes [_type].model);
		model.GetComponent<Transform> ().SetParent (this.GetComponent<Transform> ());
		model.GetComponent<Transform> ().localPosition = new Vector3 (0f, 0.5f, 0f);

		Map.UpdateCellMask (source, 0, -1);
	}

	void Update ()
	{
		// Debug Path
		int currCell = 0;
		
		while (currCell < pathDebug.Count - 1) {
			Cell startCell = pathDebug [currCell];
			Vector3 start = Map.GetWorldCoordinates (startCell);
			
			Cell endCell = pathDebug [currCell + 1];
			Vector3 end = Map.GetWorldCoordinates (endCell);
			
			Debug.DrawLine (start, end, debugPathColor);
			
			currCell++;
		}

		// Walk animation
		GetComponent<Transform> ().position = Vector3.MoveTowards (
			GetComponent<Transform> ().position,
			Map.GetWorldCoordinates (stepTarget),
			Time.deltaTime * UnitTypes [Type].speed
		);

		if (GetComponent<Transform> ().position == Map.GetWorldCoordinates (stepTarget) && go) {
			// Unit animation completed
			NextStep ();
		}
	}

	void OnMouseUp ()
	{
		Main.Select (this.GetComponent<Unit> ());
	}

	public void Select ()
	{
		model.GetComponent<Renderer> ().material.color = Color.red;
	}

	public void Deselect ()
	{
		model.GetComponent<Renderer> ().material.color = Color.white;
	}

	public void GoTo (Cell _target)
	{
		if (source != _target && stepTarget != _target) {

			target = _target;
			counter = 0;
			path = Map.FindPath (stepTarget, target);

			if (path != null) {
				// Debug
				pathDebug = new List<Cell> (path);
				debugPathColor = Color.yellow;

				// Game
				stepTarget = path [0];
				go = true;
			} else {
				// Debug
				pathDebug = new List<Cell> ();
				pathDebug.Add (stepTarget);
				pathDebug.Add (target);
				debugPathColor = Color.red;

				// Game
				go = false;

				Invoke ("TryGoTo", 1);
			}
		}
	}

	void TryGoTo ()
	{
		GoTo (target);
	}

	void NextStep ()
	{
		source = stepTarget;

		if (path != null) {
			if (path.Count == 1) {
				path = null;
				return;
			}

			if (path [1].DirectionLayers [UnitTypes [Type].DirectionGroup] == -1) {
				// The next cell is unavailable
				// TODO AI: relation between units: units go through each other or shifting if it's only the one way
				// TODO AI: Unit run away from zombies
				// TODO AI: imidietly find new path if obstacle (once)
				// TODO Pathf: One cell step: Simple A* pathfinding
				// TODO Pathf: Convert cells graph to nodes
				// TODO Pathf: Small grid
				// TODO Pathf: move by cell coordinates

				// TO DO Movement out of grid
				path = null;
				return;
			}

			if (target != source) {
				// Unit target is not achived yet
				MakeNextStep ();
			} else {
				path = null;
			}
		} else {
			// We have no path
			if (target == source) {
				pathDebug = new List<Cell> ();
				GoTo (Map.GetRandomPlace ());
			} else {
				// The array is finished, but our goal is not achieved yet
				// Probably we should wait some time, or it is not posible
				GoTo (target);
			}
		}
	}

	void MakeNextStep ()
	{
		if (counter < updatePath) {
			counter++;
			Map.UpdateCellMask (path [0], 0, 255);
		
			stepTarget = path [1];
		
			Map.UpdateCellMask (stepTarget, 0, -1);
		
			path.RemoveAt (0);
		} else {
			GetRandomUpdate ();
			GoTo (target);
		}
	}

	void GetRandomUpdate ()
	{
		updatePath = Random.Range (10, 20);
	}
}
