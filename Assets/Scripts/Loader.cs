using UnityEngine;

public class Loader : MonoBehaviour {
	public GameObject cellContainer;
	public GameObject[] terrainModels;
	public GameObject unitContainer;
	public GameObject[] ui;
	public GameObject cameraContainer;

	void Start () {
		Units.UnitContainer = unitContainer;
		Map.unitContainer = unitContainer;
		Map.CellContainer = cellContainer;
		Map.terrainModels = terrainModels;
		GameController.ui = ui;
		GameController.cameraContainer = cameraContainer;

		GameController.Init ();

	}

	void Update () {
		SwipeManager.DetectSwipe();
	}
}

