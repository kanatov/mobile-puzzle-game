using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Main : MonoBehaviour {
	public GameObject CellContainer;
	public GameObject[] TerrainModels;

	public GameObject UnitContainer;
	public UnitType[] UnitTypes;

	public GameObject[] UI;
	int[] terrain = new int[] {
		1,1,1,1,1,1,1,1,1,1,
		1,0,0,0,0,0,0,0,0,1,
		1,0,1,1,1,0,1,1,0,1,
		1,0,1,0,0,0,1,0,0,1,
		1,0,1,1,1,0,1,1,1,1,
		1,0,0,1,0,0,0,0,0,1,
		1,0,0,0,0,0,0,1,0,1,
		1,1,0,1,0,1,0,1,1,1,
		1,0,0,1,0,1,0,0,0,1,
		1,1,1,1,1,1,1,1,1,1
	};
	// Level properties
	int[] level1 = new int[] {
		10, // Width
		10, // Height
		1, // Friend units
		0, // Enemy units
		20
	};

	void Awake () {
		UnitManager.UnitTypes = UnitTypes;
		UnitManager.UnitContainer = UnitContainer;
		UnitManager.UI = UI;
		MapManager.CellContainer = CellContainer;
		MapManager.TerrainModels = TerrainModels;
		GameManager.UI = UI;

		GameManager.Init (level1, terrain);
	}

	void Update () {
//		MapManager.DebugDrawClosedCells ();
	}
}
