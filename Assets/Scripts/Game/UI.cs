using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GenericData;

public static class UI {
	public static void Init () {
		Debug.Log ("UI.Init()");

		Button newGameButton = GameController.uiMenu.GetComponent<Transform>().FindChild("New Game").GetComponent<Button>();
		newGameButton.onClick.AddListener(() => {
			Map.Init();
		});

		Button deleteButton = GameController.uiMenu.GetComponent<Transform>().FindChild("Delete").GetComponent<Button>();
		deleteButton.onClick.AddListener(() => {
			SaveLoad.Delete(SaveLoad.levelDataFileName);
			SaveLoad.Delete(SaveLoad.playerDataFileName);
		});

		GameController.uiMenu.SetActive(true);
//		input = ui[0].GetComponent<InputField>();
//		input.onEndEdit.AddListener(delegate{ChangeOverview(input);});

	}
}