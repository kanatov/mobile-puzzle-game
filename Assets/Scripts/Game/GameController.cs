using UnityEngine;
using UnityEngine.SceneManagement;
using GenericData;
using System.Collections.Generic;
using UnityEngine.UI;

public static class GameController {
	public static PlayerData playerData;
	public static GameObject GameUI;

	public static void Init ()
	{
		Debug.Log ("___Init: Loading Player data");
		LoadPlayerData ();

		if (playerData == null) 
		{
			Debug.Log ("___Init: First start");
			playerData = new PlayerData ();
			LoadScene (1);
			return;
		}

		Debug.Log ("___Init: Update playerData");
		playerData.Update ();

		Debug.Log ("___Init: Load Game Session: " + playerData.currentLevel);
		if (playerData.currentLevel != 0)
		{
			LoadScene (playerData.currentLevel);
		}
	}

	public static void LoadScene(int _scene)
	{
		Debug.Log ("___Loading scene: " + _scene);
		playerData.currentLevel = _scene;

		SavePlayerData ();
		SceneManager.LoadScene (_scene);
	}

	public static void Finish()
	{
		D.Log ("Game controller: Finish!");
		int currentScene = SceneManager.GetActiveScene ().buildIndex;
		playerData.levelsData [playerData.currentLevel].state = LevelState.Finished;
		playerData.levelsData [playerData.currentLevel].Reset ();

        if (playerData.currentLevel + 1 < Application.levelCount)
        {
            NextLevel ();
        }
        else
        {
            Exit();
        }
	}

    public static void NextLevel()
    {
        GameUI.GetComponent<Animator> ().Play ("FadeOutGameNextLevel");
    }

	public static void Exit()
	{
        D.Log("GameController.Exit()");
		GameUI.GetComponent<Animator> ().Play ("FadeOutGame");
	}

	public static void ClearPlayerData()
	{
		playerData = null;
		SaveLoad.Delete (SaveLoad.namePlayerProgress);
		Init ();
	}

	public static void SavePlayerData ()
	{
		SaveLoad.Save (playerData, SaveLoad.namePlayerProgress);
	}

	public static void LoadPlayerData ()
	{
		playerData = (PlayerData)SaveLoad.Load(SaveLoad.namePlayerProgress);
	}
}
