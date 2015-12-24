using UnityEngine;

public class MapControllerLoader : MonoBehaviour {
	void Start () {
		Debug.Log("___LOADER: Map controller");
		MapController.Init ();
	}
}
