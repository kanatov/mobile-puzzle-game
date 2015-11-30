using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Main : MonoBehaviour {
	public GameObject CellContainer;
	public GameObject[] TerrainModels;
	public GameObject UnitContainer;
	public UnitType[] UnitTypes;
	public GameObject[] UI;

	void Start () {
		Units.UnitTypes = UnitTypes;
		Units.UnitContainer = UnitContainer;
		Map.CellContainer = CellContainer;
		Map.TerrainModels = TerrainModels;
		GameController.UI = UI;

		GameController.Init ();

	}

	void Update () {
		SwipeManager.DetectSwipe();
	}
}

