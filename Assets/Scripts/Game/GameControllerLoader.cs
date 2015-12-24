using UnityEngine;

public class GameControllerLoader : MonoBehaviour {
	void Start () {
		Debug.Log("___LOADER: Game controller");
		GameController.Init ();
	}
}