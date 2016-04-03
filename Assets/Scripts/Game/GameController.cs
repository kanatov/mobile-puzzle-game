using UnityEngine;
using UnityEngine.SceneManagement;
using GenericData;
using System.Collections.Generic;

public static class GameController {
	public static PlayerData playerData;

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
		int currentScene = SceneManager.GetActiveScene ().buildIndex;
		playerData.levelsData[playerData.currentLevel].state = LevelState.Finished;
		LoadScene (0);
	}

	public static void ClearPlayerData()
	{
		SaveLoad.Delete (SaveLoad.namePlayerProgress);
		LoadScene (0);
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
