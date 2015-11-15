using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Main : MonoBehaviour
{

	//Public data
	public GameObject Map;
	public Unit selectedObject;
	public GameObject MapPrefab;

	// Level properties
	int[] level1 = new int[] {
		30, // Width
		30, // Height
		3, // Friend units
		15, // Enemy units
		80, // Grass space
	};

	void Start ()
	{
		NewLevel (level1);
	}	

	// Create Map
	void NewLevel (int[] _level)
	{

		// Looking for the old MapRoot instance and remove it
		GameObject[] mapInstances = GameObject.FindGameObjectsWithTag ("Map");
		foreach (var i in mapInstances) {
			GameObject.Destroy (i);
		}

		// Creating new map
		Map = GameObject.Instantiate (MapPrefab);
		Map MapClass = Map.GetComponent<Map> ();

		MapClass.Main = this.GetComponent<Main> ();
		MapClass.GetComponent<Map> ().Init (_level);
	}

	void Update ()
	{
		DebugDrawClosedCells ();
	}

	public void CellClick (Cell _target)
	{
		if (selectedObject) {
			selectedObject.GetComponent<Unit> ().GoTo (_target);
		}
	}

	// Selected unit
	public void Select (Unit _selected)
	{
		if (selectedObject == _selected) {
			selectedObject = null;
			_selected.Deselect ();
		} else {
			if (selectedObject != null) {
				selectedObject.Deselect ();
			}
			selectedObject = _selected;
			_selected.Select ();
		}
	}

	void DebugDrawClosedCells ()
	{
		for (int x = 0; x < Map.GetComponent<Map>().Cells.GetLength (0); x++) {
			for (int y = 0; y < Map.GetComponent<Map>().Cells.GetLength (1); y++) {
				Cell cells = Map.GetComponent<Map> ().Cells [x, y].GetComponent<Cell> ();
				if (cells.terrain == 0) {
					if (cells.DirectionLayers [1] != -1) {
						cells.Normal ();
					} else {
						cells.Highlite (0.6f, 0.6f, 0.3f);
					}
				}
			}
		}
	}
}
