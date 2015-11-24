using UnityEngine;
using System.Collections;

public class PlayerDetails : MonoBehaviour
{
	public Material archerMat;
	public Material barbarianMat;
	public Material knightMat;

	void Start ()
	{
		//PlayerPrefs.SetString("CharacterSelected","Archer");

		switch (PlayerPrefs.GetString ("CharacterSelected"))
		{
		case "Archer":
			this.renderer.material = archerMat;
			break;
		case "Barbarian":
			this.renderer.material = barbarianMat;
			break;
		case "Knight":
			this.renderer.material = knightMat;
			break;
		}
	}

	void Update ()
	{
		
	}
}
