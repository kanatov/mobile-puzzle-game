using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Loader : MonoBehaviour {
	public GameObject CellContainer;
	public GameObject[] TerrainModels;
	public GameObject UnitContainer;
	public GameObject[] UI;

	void Start () {
		Units.UnitContainer = UnitContainer;
		Map.UnitContainer = UnitContainer;
		Map.CellContainer = CellContainer;
		Map.TerrainModels = TerrainModels;
		GameController.UI = UI;

		GameController.Init ();

	}

	void Update () {
		SwipeManager.DetectSwipe();
	}
}

