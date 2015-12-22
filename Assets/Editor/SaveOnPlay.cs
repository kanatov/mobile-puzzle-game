using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SaveOnPlay
{
	static SaveOnPlay()
	{
		EditorApplication.playmodeStateChanged += SaveCurrentScene;
	}

	static void SaveCurrentScene()
	{
		if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
		{
			#if UNITY_5_3
//			Debug.Log("Saving open scenes on play...");
			UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
			#else
			Debug.Log("Saving scene on play...");
			EditorApplication.SaveScene();
			#endif
		}
	}
}