using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelButtons : MonoBehaviour {
	public GameObject[] buttons;

	void Update ()
	{
		D.Log ("UI Buttons: " + buttons.Length);
		if (buttons.Length == 0) {return;}
		if (GameController.playerData == null) {return;}

		for (int i = 0; i < Application.levelCount; i++)
		{
			if (i == 0) {continue;}

			if (i == 1)
			{
				buttons [i].GetComponent<Button> ().interactable = true;
			} else if (GameController.playerData.levelsData [i - 1].state == LevelState.Finished)
			{
				buttons [i].GetComponent<Button> ().interactable = true;
			}

			if (GameController.playerData.levelsData [i].state == LevelState.Finished)
			{
				buttons [i].GetComponent<Button> ().interactable = true;
				buttons [i].GetComponent<Image> ().color = new Color (0.8f, 1f, 0.8f, 1f);
			}
		}

		this.enabled = false;
	}
}
