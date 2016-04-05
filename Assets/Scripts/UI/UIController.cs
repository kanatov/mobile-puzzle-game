using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {
	public void LoadLevel(int _l)
	{
		GameController.LoadScene (_l);
	}

	public void Reset ()
	{
		GameController.ClearPlayerData ();
	}

	public void Exit ()
	{
		GameController.Exit ();
	}

    public void Restart()
    {
        GameController.RestertLevel();
    }

    public void NextLevel()
    {
        GameController.LoadScene(GameController.playerData.currentLevel + 1);
    }

}
