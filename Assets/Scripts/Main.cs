using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Main : MonoBehaviour {
	public GameObject CellContainer;
	public GameObject[] TerrainModels;

	public GameObject UnitContainer;
	public UnitType[] UnitTypes;

	// Level properties
	int[] level1 = new int[] {
		100, // Width
		100, // Height
		1, // Friend units
		35, // Enemy units
		80, // Grass space
	};

	void Awake () {
		UnitManager.UnitTypes = UnitTypes;
		UnitManager.UnitContainer = UnitContainer;
		MapManager.CellContainer = CellContainer;
		MapManager.TerrainModels = TerrainModels;
	}

	void Start () {
		MapManager.Init (level1);
	}	

	void Update () {
//		MapManager.DebugDrawClosedCells ();
	}
}
