using UnityEngine;

public class Loader : MonoBehaviour {
	public GameObject cellContainer;
	public GameObject mapContainer;
	public GameObject[] terrainModels;
	public UnitType[] unitTypes;
	public GameObject unitContainer;
	public GameObject[] ui;
	public GameObject cameraContainer;

	void Start () {
		Units.unitContainer = unitContainer;
		Units.unitTypes = unitTypes;
		Map.mapContainer = mapContainer;
		Map.unitContainer = unitContainer;
		Map.cellContainer = cellContainer;
		Map.terrainModels = terrainModels;
		Map.cameraContainer = cameraContainer;
		GameController.ui = ui;

		GameController.Init ();

	}

	void Update () {
		SwipeManager.DetectSwipe();
	}
}

