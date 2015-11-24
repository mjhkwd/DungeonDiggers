using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	public void startGame()
	{
		if (PlayerPrefs.GetString ("CharacterSelected")!="")
		{
			if(PlayerPrefs.GetInt("SawTutorial") == 1)
			{
				Application.LoadLevel ("_Scene_1");
			}
			else
			{
				Application.LoadLevel("tutorial");
			}
		}
		//print (0);
	}

	public void seeTut()
	{
		PlayerPrefs.SetInt ("SawTutorial",1);
		Application.LoadLevel ("tutorial");
	}

	public void sawTut()
	{
		PlayerPrefs.SetInt ("SawTutorial",1);
		if(PlayerPrefs.GetString ("CharacterSelected")!=""){
			Application.LoadLevel ("_Scene_1");
		}
		else
		Application.LoadLevel("_Scene_0");
	}

	public void endGame()
	{
		PlayerPrefs.DeleteKey("SawTutorial");
		Application.Quit();
	}

	public void newGame()
	{
		Application.LoadLevel ("_Scene_0");
	}
}
